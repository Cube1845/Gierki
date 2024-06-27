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

    public async Task RemoveGameDataFromDatabase(User user)
    {
        var gameData = await _context.TicTacToe.FindAsync(await GetGameIdByUser(user));
        _context.TicTacToe.Remove(gameData!);
        await _context.SaveChangesAsync();
    }

    public async Task<Result<GameData>> LoadGameData(User user)
    {
        GameData? gameData = await GetGameDataByUser(user);

        if (gameData == null)
        {
            return Result<GameData>.Error("User not found");
        }

        return Result<GameData>.Success(gameData, "Data sent");
    }

    public async Task<Result<GameData>> MakeMoveAndGetGameData(Move move, User user)
    {
        if (!await GameExists(user))
        {
            return Result<GameData>.Error("Game does not exist");
        }

        if (!await GetGameState(user))
        {
            return Result<GameData>.Error("Game not started");
        }

        if (move.Symbol is not "O" && move.Symbol is not "X")
        {
            return Result<GameData>.Error("Wrong symbol");
        }

        await MakeMoveInDatabaseBoard(move, user);

        if (await IsWinSituation(user))
        {
            await SetWinSituationInDatabase(move.Symbol, await GetWinningTiles(user), user);
            GameData? gameData = await GetGameDataByUser(user)!;
            return Result<GameData>.Success(gameData!, "Game ended");
        }

        if (await IsGameTied(user) && !await IsWinSituation(user))
        {
            await SetWinSituationInDatabase(true, user);
            GameData? gameData = await GetGameDataByUser(user)!;
            return Result<GameData>.Success(gameData!, "Tie");
        }

        await ChangeTurnInDataBase(move.Symbol, user);
        GameData? data = await GetGameDataByUser(user)!;
        return Result<GameData>.Success(data!, "Successfully made a move");
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

    private async Task<GameData?> GetGameDataByUser(User user)
    {
        int? id = await GetGameIdByUser(user);

        if (id == null)
        {
            return null;
        }

        var data = await _context.TicTacToe.FindAsync(id);

        if (data == null)
        {
            return null;
        }

        return GameData.FromTicTacToePersistence(data);
    }

    private async Task<int> GetGameIdByUser(User user)
    {
        int id = -1;

        var gameDb = await _context.TicTacToe.ToListAsync();

        foreach (var ttt in gameDb)
        {
            if (ttt.Players.Contains(user.Username))
            {
                id = ttt.Id;
            }
        }

        return id;
    }

    private async Task<bool> GameExists(User user)
    {
        var boardDb = await _context.TicTacToe.FindAsync((await GetGameIdByUser(user)));

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

    private async Task<bool> GetGameState(User user)
    {
        var boardDb = await _context.TicTacToe.FindAsync(await GetGameIdByUser(user));

        if (boardDb is not null)
        {
            return boardDb.IsGameStarted;
        }

        return false;
    }

    private async Task ChangeTurnInDataBase(string currentTurn, User user)
    {
        if (currentTurn == "O")
        {
            await SetTurnInDataBase("X", user);
        }
        else
        {
            await SetTurnInDataBase("O", user);
        }
    }

    private async Task SetWinSituationInDatabase(string gameWinnedBy, PositionCollection? winningTiles, User user)
    {
        var boardDb = await _context.TicTacToe.FindAsync(await GetGameIdByUser(user));

        if (boardDb is not null)
        {
            boardDb.GameWinnedBy = gameWinnedBy;
            boardDb.WinningTiles = JsonConvert.SerializeObject(winningTiles);
            boardDb.Turn = "";
            boardDb.IsGameStarted = false;
        }

        await _context.SaveChangesAsync();
    }

    private async Task SetWinSituationInDatabase(bool isGameTied, User user)
    {
        var boardDb = await _context.TicTacToe.FindAsync(await GetGameIdByUser(user));

        if (boardDb is not null)
        {
            boardDb.IsGameTied = isGameTied;
            boardDb.IsGameStarted = false;
            boardDb.Turn = "";
        }

        await _context.SaveChangesAsync();
    }

    private async Task SetTurnInDataBase(string turn, User user)
    {
        var boardDb = await _context.TicTacToe.FindAsync(await GetGameIdByUser(user));

        if (boardDb is not null)
        {
            boardDb.Turn = turn;
        }

        await _context.SaveChangesAsync();
    }

    private async Task MakeMoveInDatabaseBoard(Move move, User user)
    {
        var board = await LoadBoardFromDatabase(user);
        board.BoardData[move.Position.Y].Positions[move.Position.X] = move.Symbol;
        await SaveBoardToDatabase(board, user);
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

    private async Task SaveBoardToDatabase(Board board, User user)
    {
        var boardDb = await _context.TicTacToe.FindAsync(await GetGameIdByUser(user));
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

    private async Task<bool> IsGameTied(User user)
    {
        var board = await LoadBoardFromDatabase(user);

        return !board.BoardData.Any(row =>
            row.Positions.Any(pos => pos is "")
        );
    }

    private async Task<PositionCollection?> GetWinningTiles(User user)
    {
        var board = await LoadBoardFromDatabase(user);

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

    private async Task<bool> IsWinSituation(User user)
    {
        return await GetWinningTiles(user) != null;
    }

    private async Task<Board> LoadBoardFromDatabase(User user)
    {
        var boardDb = await _context.TicTacToe.FindAsync(await GetGameIdByUser(user));
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
