using Microsoft.AspNetCore.Routing.Constraints;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TicTacToe.Repositories
{
    public class BoardRepository
    {
        public static List<List<List<int>>> WinningCases = new List<List<List<int>>>
        {
            new List<List<int>>
            {
                new List<int> {0, 0},
                new List<int> {0, 1},
                new List<int> {0, 2},
            },
            new List<List<int>>
            {
                new List<int> {1, 0},
                new List<int> {1, 1},
                new List<int> {1, 2},
            },
            new List<List<int>>
            {
                new List<int> {2, 0},
                new List<int> {2, 1},
                new List<int> {2, 2},
            },
            new List<List<int>>
            {
                new List<int> {0, 0},
                new List<int> {1, 0},
                new List<int> {2, 0},
            },
            new List<List<int>>
            {
                new List<int> {0, 1},
                new List<int> {1, 1},
                new List<int> {2, 1},
            },
            new List<List<int>>
            {
                new List<int> {0, 2},
                new List<int> {1, 2},
                new List<int> {2, 2},
            },
            new List<List<int>>
            {
                new List<int> {0, 0},
                new List<int> {1, 1},
                new List<int> {2, 2},
            },
            new List<List<int>>
            {
                new List<int> {0, 2},
                new List<int> {1, 1},
                new List<int> {2, 0},
            },
        };

        public static string FileName = "game.json";

        public static List<List<string>> getEmptyBoard()
        {
            return new List<List<string>>{ new(){ "", "", "" },
                                           new(){ "", "", "" },
                                           new(){ "", "", "" },};
        }

        public static void setBoardFileData(string data)
        {
            File.WriteAllText(FileName, data);
        }

        public static string getSerializedBoardFileData()
        {
            return File.ReadAllText(FileName);
        }

        public static bool isGameTied()
        {
            var board = JsonSerializer.Deserialize<List<List<string>>>(getSerializedBoardFileData());
            foreach (List<string> row in board)
            {
                if (row[0] == "" || row[1] == "" || row[2] == "")
                {
                    return false;
                }
            }
            return true;
        }

        public static List<List<int>>? getWinningTiles()
        {
            var board = JsonSerializer.Deserialize<List<List<string>>>(getSerializedBoardFileData());
            int[] y = { -1, -1, -1 };
            int[] x = { -1, -1, -1 };
            foreach (List<List<int>> cases in WinningCases)
            {
                for (int i = 0; i < 3; i++)
                {
                    y[i] = cases[i][0];
                    x[i] = cases[i][1];
                }
                if (board[y[0]][x[0]] == board[y[1]][x[1]] && 
                    board[y[2]][x[2]] == board[y[1]][x[1]] &&
                    board[y[0]][x[0]] != "")
                {
                    return cases;
                }
            }
            return null;
        }
    }
}
