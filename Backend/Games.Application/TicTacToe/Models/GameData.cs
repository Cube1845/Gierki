using Games.Application.Persistence;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Application.TicTacToe.Models
{
    public class GameData(Board board, bool isGameStarted, string? gameWinnedBy, bool isGameTied, string turn, PositionCollection? winningTiles)
    {
        public Board Board { get; set; } = board;
        public bool IsGameStarted { get; set; } = isGameStarted;
        public string? GameWinnedBy { get; set; } = gameWinnedBy;
        public bool IsGameTied { get; set; } = isGameTied;
        public string Turn { get; set; } = turn;
        public PositionCollection? WinningTiles { get; set; } = winningTiles;

        public static GameData FromTicTacToe(Persistence.TicTacToe ticTacToe)
        {
            return new GameData(
                JsonConvert.DeserializeObject<Board>(ticTacToe.Board),
                ticTacToe.IsGameStarted,
                ticTacToe.GameWinnedBy,
                ticTacToe.IsGameTied,
                ticTacToe.Turn,
                JsonConvert.DeserializeObject<PositionCollection>(ticTacToe.WinningTiles)
            );
        }
    }

    public class GameData1(Board board, bool isGameStarted, bool isGameTied, string turn, string? gameWinnedBy = null, PositionCollection? winningTiles = null)
    {
        public Board Board { get; set; } = board;
        public bool IsGameStarted { get; set; } = isGameStarted;
        public string? GameWinnedBy { get; set; } = gameWinnedBy;
        public bool IsGameTied { get; set; } = isGameTied;
        public string Turn { get; set; } = turn;
        public PositionCollection? WinningTiles { get; set; } = winningTiles;
    }
}
