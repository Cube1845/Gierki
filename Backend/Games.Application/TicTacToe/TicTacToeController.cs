using Games.Application.Infrastructure;
using Games.Application.TicTacToe.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Games.Application.TicTacToe;

[ApiController]
[Route("Move")]
public class TicTacToeController(TicTacToeService ticTacToeService) : Controller
{
    private readonly TicTacToeService _ticTacToeService = ticTacToeService;

    [HttpGet("[action]")]
    public IActionResult StartGame()
    {
        var board = _ticTacToeService.GetEmptyBoard();
        string jsonString = JsonSerializer.Serialize(board);

        _ticTacToeService.SetBoardFileData(jsonString);

        var response = Result.Success("Game started");
        return Ok(response);
    }

    [HttpPost("[action]")]
    public IActionResult MakeMoveAndCheckGameStatus([FromBody] Move move)
    {
        string jsonString = _ticTacToeService.GetSerializedBoardFileData();
        var board = JsonSerializer.Deserialize<List<List<string>>>(jsonString);

        if (move.Position != null && (move.Symbol == "O" || move.Symbol == "X"))
        {
            int y = move.Position[0];
            int x = move.Position[1];

            board[y][x] = move.Symbol;
            jsonString = JsonSerializer.Serialize(board);

            _ticTacToeService.SetBoardFileData(jsonString);

            if (_ticTacToeService.GetWinningTiles() != null)
            {
                var result = Result<List<List<string>>>.Success(board, "Game ended");
                return Ok(result);
            }

            if (_ticTacToeService.IsGameTied())
            {
                if (_ticTacToeService.GetWinningTiles() == null)
                {
                    var result = Result<List<List<string>>>.Success(board, "Tie");
                    return Ok(result);
                }
            }
            else
            {
                var result = Result<List<List<string>>>.Success(board, "Successfully made a move");
                return Ok(result);
            }
        }

        var response = Result<List<List<string>>>.Error("Error");
        return Ok(response);
    }
}
