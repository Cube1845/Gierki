using Games.Application.Infrastructure;
using Games.Application.TicTacToe.Models;
using Microsoft.AspNetCore.Mvc;

namespace Games.Application.TicTacToe;

[ApiController]
[Route("Move")]
public class TicTacToeController(TicTacToeService ticTacToeService) : Controller
{
    private readonly TicTacToeService _ticTacToeService = ticTacToeService;

    [HttpGet("[action]")]
    public IActionResult StartGame()
    {
        _ticTacToeService.StartGame();

        var response = Result.Success("Game started");
        return Ok(response);
    }

    [HttpPost("[action]")]
    public IActionResult MakeMoveAndCheckGameStatus([FromBody] Move move)
    {
        var response = _ticTacToeService.MakeMove(move);
        return Ok(response);
    }
}
