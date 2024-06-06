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

        return Ok(new Response("Game started"));
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
                return Ok(new Response("Game ended", board));
            }

            if (_ticTacToeService.IsGameTied())
            {
                if (_ticTacToeService.GetWinningTiles() == null)
                {
                    return Ok(new Response("Tie", board));
                }
            }

            return Ok(new Response("Successfully made a move", board));
        }

        return Ok(new Response("Error"));
    }
}
