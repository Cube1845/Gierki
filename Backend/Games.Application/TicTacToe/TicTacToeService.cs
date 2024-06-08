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
        await SaveBoard(board);
    }

    public async Task<Result<List<List<string>>>> MakeMove(Move move)
    {
        var board = LoadBoardFromFile();

        if (move.Symbol is "O" || move.Symbol is "X")
        {
            board[move.Position.Y][move.Position.X] = move.Symbol;

            await SaveBoard(board);

            if (GetWinningTiles() != null)
            {
                return Result<List<List<string>>>.Success(board, "Game ended");
            }

            if (IsGameTied() && !IsWinSituation())
            {
                return Result<List<List<string>>>.Success(board, "Tie");
            }

            return Result<List<List<string>>>.Success(board, "Successfully made a move");
        }

        return Result<List<List<string>>>.Error("Error"); ;
    }

    private List<List<string>> GetEmptyBoard()
    {
        return
        [
            ["", "", ""],
            ["", "", ""],
            ["", "", ""],
        ];
    }

    private async Task SaveBoard(List<List<string>> board)
    {
        var serializedBoard = JsonSerializer.Serialize(board);
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

    private bool IsGameTied()
    {
        var board = LoadBoardFromFile();

        foreach (List<string> row in board)
        {
            if (row.Any(x => x is ""))
            {
                return false;
            }
        }

        return true;
    }

    private PositionCollection? GetWinningTiles()
    {
        var board = LoadBoardFromFile();

        foreach (var winningCase in WinningCases)
        {
            var tile1 = board[winningCase.T1.Y][winningCase.T1.X];
            var tile2 = board[winningCase.T2.Y][winningCase.T2.X];
            var tile3 = board[winningCase.T3.Y][winningCase.T3.X];

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

    private List<List<string>> LoadBoardFromFile()
    {
        var fileText = File.ReadAllText(_fileName);
        return JsonSerializer.Deserialize<List<List<string>>>(fileText)!;
    }
}
