using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text.Json;
using TicTacToe.Models;
using TicTacToe.Repositories;

namespace TicTacToe.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MoveController : Controller
    {
        [HttpGet("[action]")]
        public IActionResult StartGameAndGetBoard()
        {
            var board = BoardRepository.getEmptyBoard();
            string jsonString = JsonSerializer.Serialize(board);
            BoardRepository.setBoardFileData(jsonString);
            return Ok(jsonString + BoardRepository.getWinningTiles());
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
                    return Ok("Game ended");
                }
                if (BoardRepository.isGameTied())
                {
                    if (BoardRepository.getWinningTiles() == null)
                    {
                        return Ok("Tie");
                    }
                }
                return Ok("Successfully made a move");
            }
            return Ok("Error");
        }
    }
}
