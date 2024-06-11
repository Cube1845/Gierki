using Games.Application.Infrastructure;
using Games.Application.Persistence;
using Games.Application.TicTacToe.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Games.Application.TicTacToe.Services;

public class TicTacToeService(GamesDbContext context)
{
    private readonly GamesDbContext _context = context;

    private readonly List<PositionCollection> WinningCases =
    [
        new([0, 0], [0, 1], [0, 2]),
        new([1, 0], [1, 1], [1, 2]),
        new([2, 0], [2, 1], [2, 2]),
        new([0, 0], [1, 0], [2, 0]),
        new([0, 1], [1, 1], [2, 1]),
        new([0, 2], [1, 2], [2, 2]),
        new([0, 0], [1, 1], [2, 2]),
        new([0, 2], [1, 1], [2, 0]),
    ];

    public async Task StartGame()
    {
        var board = GetEmptyBoard();
        await SaveBoardToDatabase(board);

    }

    public async Task<Result<GameData>> DeserializeStringAndMakeMove(string text)
    {
        if (await GetGameState())
        {
            var move = JsonConvert.DeserializeObject<Move>(text);
            return await MakeMoveAndGetGameData(move!);
        }

        return Result<GameData>.Error("Game not started");
    }

    private async Task SaveGameDataToDatabase(GameData gameData)
    {
        var serializedBoard = JsonConvert.SerializeObject(board);
        var boardDb = await _context.TicTacToe.FirstOrDefaultAsync();

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

    private async Task<bool> GetGameState()
    {
        var boardDb = await _context.TicTacToe.FirstOrDefaultAsync();

        if (boardDb is not null)
        {
            return boardDb.IsGameStarted;
        }

        return false;
    }

    private async Task<GameData> GetGameData()
    {
        var boardDb = await _context.TicTacToe.FirstOrDefaultAsync();
        GameData gameData;

        if (boardDb is not null)
        {
            gameData = new GameData(
                JsonConvert.DeserializeObject<Board>(boardDb.Board),
                boardDb.IsGameStarted,
                boardDb.GameWinnedBy,
                boardDb.IsGameTied,
                boardDb.Turn,
                JsonConvert.DeserializeObject<PositionCollection>(boardDb.WinningTiles)
            );
        }
        else
        {
            throw new ArgumentException("Got no data from database");
        }

        return gameData;
    }

    private async Task<Result<GameData>> MakeMoveAndGetGameData(Move move)
    {
        if (move.Symbol is "O" || move.Symbol is "X")
        {
            await MakeMoveInDatabaseBoard(move);

            if (await IsWinSituation())
            {
                await SetWinSituationInDatabase(move.Symbol, await GetWinningTiles()!);
                return Result<GameData>.Success(await GetGameData(), "Game ended");
            }

            if (await IsGameTied() && !await IsWinSituation())
            {
                await SetWinSituationInDatabase(true);
                return Result<GameData>.Success(await GetGameData(), "Tie");
            }

            await ChangeTurnInDataBase(move.Symbol);
            return Result<GameData>.Success(await GetGameData(), "Successfully made a move");
        }

        return Result<GameData>.Error("Error");
    }

    private async Task ChangeTurnInDataBase(string currentTurn)
    {
        if (currentTurn == "O")
        {
            await SetTurnInDataBase("X");
        }
        else
        {
            await SetTurnInDataBase("O");
        }
    }

    private async Task SetWinSituationInDatabase(string gameWinnedBy, PositionCollection winningTiles)
    {
        var boardDb = await _context.TicTacToe.FirstOrDefaultAsync();

        if (boardDb is not null)
        {
            boardDb.GameWinnedBy = gameWinnedBy;
            boardDb.WinningTiles = JsonConvert.SerializeObject(winningTiles);
        }

        await _context.SaveChangesAsync();
    }

    private async Task SetWinSituationInDatabase(bool isGameTied)
    {
        var boardDb = await _context.TicTacToe.FirstOrDefaultAsync();

        if (boardDb is not null)
        {
            boardDb.IsGameTied = isGameTied;
        }

        await _context.SaveChangesAsync();
    }

    private async Task SetTurnInDataBase(string turn)
    {
        var boardDb = await _context.TicTacToe.FirstOrDefaultAsync();

        if (boardDb is not null)
        {
            boardDb.Turn = turn;
        }

        await _context.SaveChangesAsync();
    }

    private async Task MakeMoveInDatabaseBoard(Move move)
    {
        var board = await LoadBoardFromDatabase();
        board.BoardData[move.Position.Y].Positions[move.Position.X] = move.Symbol;
        await SaveBoardToDatabase(board);
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

    private async Task SaveBoardToDatabase(Board board)
    {
        var serializedBoard = JsonConvert.SerializeObject(board);
        var boardDb = await _context.TicTacToe.FirstOrDefaultAsync();

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

    private async Task<bool> IsGameTied()
    {
        var board = await LoadBoardFromDatabase();

        foreach (BoardRow row in board.BoardData)
        {
            if (row.Positions.Any(x => x is ""))
            {
                return false;
            }
        }

        return true;
    }

    private async Task<PositionCollection?> GetWinningTiles()
    {
        var board = await LoadBoardFromDatabase();

        foreach (var winningCase in WinningCases)
        {
            var tile1 = board.BoardData[winningCase.T1.Y].Positions[winningCase.T1.X];
            var tile2 = board.BoardData[winningCase.T2.Y].Positions[winningCase.T2.X];
            var tile3 = board.BoardData[winningCase.T3.Y].Positions[winningCase.T3.X];

            string[] tiles = [tile1, tile2, tile3];

            var isWinningCase =
                tiles.All(t => !string.IsNullOrEmpty(t)) &&
                (tiles.All(t => t == "X") || tiles.All(t => t == "O"));

            if (isWinningCase)
            {
                return winningCase;
            }
        }

        return null;
    }

    private async Task<bool> IsWinSituation()
    {
        return await GetWinningTiles() != null;
    }

    private async Task<Board> LoadBoardFromDatabase()
    {
        var boardDb = await _context.TicTacToe.FirstOrDefaultAsync();
        string jsonText = "";

        if (boardDb is not null)
        {
            jsonText = boardDb.Board;
        }
        else
        {
            throw new ArgumentException("Got no data from database");
        }

        return JsonConvert.DeserializeObject<Board>(jsonText)!;
    }
}
