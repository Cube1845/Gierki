using Games.Application.Infrastructure;
using Games.Application.Persistence;
using Games.Application.TicTacToe.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Games.Application.TicTacToe.Services;

public class TicTacToeService(GamesDbContext context)
{
    private readonly GamesDbContext _context = context;

    private readonly List<PositionCollection> WinningCases =
    [
        new([new(0, 0), new(0, 1), new(0, 2)]),
        new([new(1, 0), new(1, 1), new(1, 2)]),
        new([new(2, 0), new(2, 1), new(2, 2)]),
        new([new(0, 0), new(1, 0), new(2, 0)]),
        new([new(0, 1), new(1, 1), new(2, 1)]),
        new([new(0, 2), new(1, 2), new(2, 2)]),
        new([new(0, 0), new(1, 1), new(2, 2)]),
        new([new(0, 2), new(1, 1), new(2, 0)])
    ];

    public async Task<Result<GameData>> LoadGameDataAndUpdateConnectionIds(string oldConnectionId, string connectionId)
    {
        GameData? gameData = await UpdateConnectionIdAndGetGameData(oldConnectionId, connectionId);
        if (gameData == null)
        {
            return Result<GameData>.Error("User not found");
        }

        return Result<GameData>.Success(gameData, "Data sent");
    }

    private async Task<GameData?> UpdateConnectionIdAndGetGameData(string oldConnectionId, string connectionId)
    {
        int? id = await GetGameIdByConnectionId(oldConnectionId);
        if (id == null)
        {
            return null;
        }

        var data = await _context.TicTacToe.FindAsync(id);
        var oldPlayers = JsonConvert.DeserializeObject<List<UserRole>>(data!.Players);
        oldPlayers.Find(player => player.User.ConnectionId == oldConnectionId)!.User.ConnectionId = connectionId;
        data.Players = JsonConvert.SerializeObject(oldPlayers);

        await _context.SaveChangesAsync();

        if (data != null)
        {
            return GameData.FromTicTacToePersistence(data!);
        }
        return null;
    }

    private async Task<int> GetGameIdByConnectionId(string connectionId)
    {
        int id = -1;

        var gameDb = await _context.TicTacToe.ToListAsync();
        foreach (var ttt in gameDb)
        {
            if (ttt.Players.Contains(connectionId))
            {
                id = ttt.Id;
            }
        }

        return id;
    }

    public async Task<GameData> StartGame(List<User> users)
    {
        var board = GetEmptyBoard();

        List<UserRole> usersWithRoles;
        Random random = new Random();
        if (random.Next(0, 2) == 0)
        {
            usersWithRoles = new List<UserRole>()
            {
                new(users[0], "O"),
                new(users[1], "X")
            };
        }
        else
        {
            usersWithRoles = new List<UserRole>()
            {
                new(users[1], "O"),
                new(users[0], "X")
            };
        }

        GameData gameData = new(
            board,
            isGameStarted: true,
            isGameTied: false,
            "O",
            usersWithRoles
        );

        await SaveGameDataToDatabase(gameData);

        return gameData;
    }

    public async Task<Result<GameData>> MakeMoveAndGetGameData(Move move, string connectionId)
    {   
        if (!await gameExists(connectionId))
        {
            return Result<GameData>.Error("Game does not exist");
        }

        if (!await GetGameState(connectionId))
        {
            return Result<GameData>.Error("Game not started");
        }

        if (move.Symbol is not "O" && move.Symbol is not "X")
        {
            return Result<GameData>.Error("Wrong symbol");
        }

        await MakeMoveInDatabaseBoard(move, connectionId);

        if (await IsWinSituation(connectionId))
        {
            await SetWinSituationInDatabase(move.Symbol, await GetWinningTiles(connectionId), connectionId);
            return Result<GameData>.Success(await GetGameDataByUsersConnectionId(connectionId), "Game ended");
        }

        if (await IsGameTied(connectionId) && !await IsWinSituation(connectionId))
        {
            await SetWinSituationInDatabase(true, connectionId);
            return Result<GameData>.Success(await GetGameDataByUsersConnectionId(connectionId), "Tie");
        }

        await ChangeTurnInDataBase(move.Symbol, connectionId);
        return Result<GameData>.Success(await GetGameDataByUsersConnectionId(connectionId), "Successfully made a move");
    }

    private async Task<bool> gameExists(string connectionId)
    {
        var boardDb = await _context.TicTacToe.FindAsync((await GetGameIdByConnectionId(connectionId)));
        if (boardDb == null)
        {
            return false;
        }

        return true;
    }

    private async Task SaveGameDataToDatabase(GameData gameData)
    {
        var serializedBoard = JsonConvert.SerializeObject(gameData.Board);
        string serializedWinningTiles = "";

        if (gameData.WinningTiles != null)
        {
            serializedWinningTiles = JsonConvert.SerializeObject(gameData.WinningTiles);
        }

        var ticTacToe = new Persistence.TicTacToe()
        {
            Board = serializedBoard,
            IsGameStarted = gameData.IsGameStarted,
            GameWinnedBy = gameData.GameWinnedBy,
            IsGameTied = gameData.IsGameTied,
            Turn = gameData.Turn,
            WinningTiles = serializedWinningTiles,
            Players = JsonConvert.SerializeObject(gameData.Players)
        };
        await _context.TicTacToe.AddAsync(ticTacToe);

        await _context.SaveChangesAsync();
    }

    private async Task<bool> GetGameState(string connectionId)
    {
        var boardDb = await _context.TicTacToe.FindAsync(await GetGameIdByConnectionId(connectionId));

        if (boardDb is not null)
        {
            return boardDb.IsGameStarted;
        }

        return false;
    }

    private async Task<GameData> GetGameDataByUsersConnectionId(string connectionId)
    {
        var data = await _context.TicTacToe.FindAsync(await GetGameIdByConnectionId(connectionId));
        return GameData.FromTicTacToePersistence(data!);
    }

    private async Task ChangeTurnInDataBase(string currentTurn, string connectionId)
    {
        if (currentTurn == "O")
        {
            await SetTurnInDataBase("X", connectionId);
        }
        else
        {
            await SetTurnInDataBase("O", connectionId);
        }
    }

    private async Task SetWinSituationInDatabase(string gameWinnedBy, PositionCollection? winningTiles, string connectionId)
    {
        var boardDb = await _context.TicTacToe.FindAsync(await GetGameIdByConnectionId(connectionId));

        if (boardDb is not null)
        {
            boardDb.GameWinnedBy = gameWinnedBy;
            boardDb.WinningTiles = JsonConvert.SerializeObject(winningTiles);
            boardDb.Turn = "";
            boardDb.IsGameStarted = false;
        }

        await _context.SaveChangesAsync();
    }

    private async Task SetWinSituationInDatabase(bool isGameTied, string connectionId)
    {
        var boardDb = await _context.TicTacToe.FindAsync(await GetGameIdByConnectionId(connectionId));

        if (boardDb is not null)
        {
            boardDb.IsGameTied = isGameTied;
            boardDb.IsGameStarted = false;
            boardDb.Turn = "";
        }

        await _context.SaveChangesAsync();
    }

    private async Task SetTurnInDataBase(string turn, string connectionId)
    {
        var boardDb = await _context.TicTacToe.FindAsync(await GetGameIdByConnectionId(connectionId));

        if (boardDb is not null)
        {
            boardDb.Turn = turn;
        }

        await _context.SaveChangesAsync();
    }

    private async Task MakeMoveInDatabaseBoard(Move move, string connectionId)
    {
        var board = await LoadBoardFromDatabase(connectionId);
        board.BoardData[move.Position.Y].Positions[move.Position.X] = move.Symbol;
        await SaveBoardToDatabase(board, connectionId);
    }

    private Board GetEmptyBoard()
    {
        return
        new([
            new(["", "", ""]),
            new(["", "", ""]),
            new(["", "", ""])
        ]);
    }

    private async Task SaveBoardToDatabase(Board board, string connectionId)
    {
        var boardDb = await _context.TicTacToe.FindAsync(await GetGameIdByConnectionId(connectionId));
        var serializedBoard = JsonConvert.SerializeObject(board);

        if (boardDb is not null)
        {
            boardDb.Board = serializedBoard;
        }
        else
        {
            var ticTacToe = new Persistence.TicTacToe()
            {
                Board = serializedBoard,
            };

            await _context.TicTacToe.AddAsync(ticTacToe);
        }

        await _context.SaveChangesAsync();
    }

    private async Task<bool> IsGameTied(string connectionId)
    {
        var board = await LoadBoardFromDatabase(connectionId);

        return !board.BoardData.Any(row =>
            row.Positions.Any(pos => pos is "")
        );
    }

    private async Task<PositionCollection?> GetWinningTiles(string connectionId)
    {
        var board = await LoadBoardFromDatabase(connectionId);

        foreach (var winningCase in WinningCases)
        {
            var tile1 = board.BoardData[winningCase.Positions[0].Y].Positions[winningCase.Positions[0].X];
            var tile2 = board.BoardData[winningCase.Positions[1].Y].Positions[winningCase.Positions[1].X];
            var tile3 = board.BoardData[winningCase.Positions[2].Y].Positions[winningCase.Positions[2].X];

            string[] tiles = [ tile1, tile2, tile3 ];

            var isWinningCase = 
                tiles.All(t => t == "X") || 
                tiles.All(t => t == "O");

            if (isWinningCase)
            {
                return winningCase;
            }
        }

        return null;
    }

    private async Task<bool> IsWinSituation(string connectionId)
    {
        return await GetWinningTiles(connectionId) != null;
    }

    private async Task<Board> LoadBoardFromDatabase(string connectionId)
    {
        var boardDb = await _context.TicTacToe.FindAsync(await GetGameIdByConnectionId(connectionId));
        string jsonText = "";

        if (boardDb is not null)
        {
            jsonText = boardDb.Board;
        }
        else
        {
            return GetEmptyBoard();
        }

        return JsonConvert.DeserializeObject<Board>(jsonText)!;
    }
}
