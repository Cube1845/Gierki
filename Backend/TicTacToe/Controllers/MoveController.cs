using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TicTacToe.Models;
using TicTacToe.Repositories;

namespace TicTacToe.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MoveController(BoardRepository boardRepository) : Controller
    {
        private readonly BoardRepository _boardRepository = boardRepository;

        [HttpGet("[action]")]
        public IActionResult StartGame()
        {
            var board = _boardRepository.GetEmptyBoard();
            string jsonString = JsonSerializer.Serialize(board);

            _boardRepository.SetBoardFileData(jsonString);

            return Ok(new Response("Game started"));
        }

        [HttpPost("[action]")]
        public IActionResult MakeMoveAndCheckGameStatus([FromBody] Move move)
        {
            string jsonString = _boardRepository.GetSerializedBoardFileData();
            var board = JsonSerializer.Deserialize<List<List<string>>>(jsonString);

            if (move.Position != null && (move.Symbol == "O" || move.Symbol == "X"))
            {
                int y = move.Position[0];
                int x = move.Position[1];

                board[y][x] = move.Symbol;
                jsonString = JsonSerializer.Serialize(board);

                _boardRepository.SetBoardFileData(jsonString);

                if (_boardRepository.GetWinningTiles() != null)
                {
                    return Ok(new Response("Game ended", board));
                }

                if (_boardRepository.IsGameTied())
                {
                    if (_boardRepository.GetWinningTiles() == null)
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
