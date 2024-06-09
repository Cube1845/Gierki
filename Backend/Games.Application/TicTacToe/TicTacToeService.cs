using Games.Application.Infrastructure;
using Games.Application.Persistence;
using Games.Application.TicTacToe.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Games.Application.TicTacToe;

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

    private readonly string _fileName = "game.json";

    public async Task StartGame()
    {
        var board = GetEmptyBoard();
        await SaveBoardToDatabase(board);
    }

    public async Task<Result<Board>> MakeMove(Move move)
    {
        var board = await LoadBoardFromDatabase();

        if (move.Symbol is "O" || move.Symbol is "X")
        {
            board.BoardData[move.Position.Y].Positions[move.Position.X] = move.Symbol;

            await SaveBoardToDatabase(board);

            if (IsWinSituation())
            {
                return Result<Board>.Success(board, "Game ended");
            }

            if (await IsGameTied() && !IsWinSituation())
            {
                return Result<Board>.Success(board, "Tie");
            }

            return Result<Board>.Success(board, "Successfully made a move");
        }

        return Result<Board>.Error("Error"); ;
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
        var listedBoard = Board.ConvertFromBoardModelToMultiList(board);
        var serializedBoard = JsonSerializer.Serialize(listedBoard);
        var boardDb = await _context.TicTacToe.FirstOrDefaultAsync();

        if (boardDb is not null)
        {
            boardDb.Board = serializedBoard;
        }
        else
        {
            var ticTacToe = new Application.Persistence.TicTacToe() 
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

    private bool IsWinSituation()
    {
        return GetWinningTiles() != null;
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

        var list = JsonSerializer.Deserialize<List<List<string>>>(jsonText)!;
        return Board.ConvertFromMultiListToBoardModel(list);
    }
}
