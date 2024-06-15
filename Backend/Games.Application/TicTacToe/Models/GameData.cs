using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Application.TicTacToe.Models
{
    public class GameData(Board board, 
                          bool isGameStarted,
                          bool isGameTied,
                          string turn,
                          List<UserRole> players,
                          int? id = null,
                          string? gameWinnedBy = null,
                          PositionCollection? winningTiles = null)
    {
        public int? Id { get; set; } = id;
        public Board Board { get; set; } = board;
        public bool IsGameStarted { get; set; } = isGameStarted;
        public string? GameWinnedBy { get; set; } = gameWinnedBy;
        public bool IsGameTied { get; set; } = isGameTied;
        public string Turn { get; set; } = turn;
        public PositionCollection? WinningTiles { get; set; } = winningTiles;
        public List<UserRole> Players { get; set; } = players;

        public static GameData FromTicTacToePersistence(Persistence.TicTacToe ticTacToe)
        {
            return new GameData(
                JsonConvert.DeserializeObject<Board>(ticTacToe.Board),
                ticTacToe.IsGameStarted,
                ticTacToe.IsGameTied,
                ticTacToe.Turn,
                JsonConvert.DeserializeObject<List<UserRole>>(ticTacToe.Players),
                ticTacToe.Id,
                ticTacToe.GameWinnedBy,
                JsonConvert.DeserializeObject<PositionCollection>(ticTacToe.WinningTiles)
            );
        }
    }
}
