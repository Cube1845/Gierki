using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Web.Http.Cors;
using TicTacToe.Models;
using TicTacToe.Repositories;

namespace TicTacToe.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]
    public class MoveController : Controller
    {
        [HttpGet("[action]")]
        public IActionResult StartGame()
        {
            var board = BoardRepository.getEmptyBoard();
            string jsonString = JsonSerializer.Serialize(board);
            BoardRepository.setBoardFileData(jsonString);
            return Ok(new Response("Game started"));
        }

        [HttpPost("[action]")]
        public IActionResult MakeMoveAndCheckGameStatus([FromBody] Move move)
        {
            string jsonString = BoardRepository.getSerializedBoardFileData();
            var board = JsonSerializer.Deserialize<List<List<string>>>(jsonString);
            if (move.Position != null && (move.Symbol == "O" || move.Symbol == "X"))
            {
                int y = move.Position[0];
                int x = move.Position[1];
                board[y][x] = move.Symbol;
                jsonString = JsonSerializer.Serialize(board);
                BoardRepository.setBoardFileData(jsonString);
                if (BoardRepository.getWinningTiles() != null)
                {
                    return Ok(new Response("Game ended", board));
                }
                if (BoardRepository.isGameTied())
                {
                    if (BoardRepository.getWinningTiles() == null)
                    {
                        return Ok(new Response("Tie", board));
                    }
                }
                return Ok(new Response("Successfully made a move", board));
            }
            return Ok(new Response("Error"));
        }
    }
}
