using System.Text.Json;

namespace Games.Application.TicTacToe;

public class TicTacToeService
{
    public List<List<List<int>>> WinningCases =
        [
            [
                [0, 0],
                [0, 1],
                [0, 2],
            ],
            [
                [1, 0],
                [1, 1],
                [1, 2],
            ],
            [
                [2, 0],
                [2, 1],
                [2, 2],
            ],
            [
                [0, 0],
                [1, 0],
                [2, 0],
            ],
            [
                [0, 1],
                [1, 1],
                [2, 1],
            ],
            [
                [0, 2],
                [1, 2],
                [2, 2],
            ],
            [
                [0, 0],
                [1, 1],
                [2, 2],
            ],
            [
                [0, 2],
                [1, 1],
                [2, 0]
            ]
        ];

    public string FileName = "game.json";

    public List<List<string>> GetEmptyBoard()
    {
        return
        [
            ["", "", ""],
                ["", "", ""],
                ["", "", ""],
            ];
    }

    public void SetBoardFileData(string data)
    {
        File.WriteAllText(FileName, data);
    }

    public string GetSerializedBoardFileData()
    {
        return File.ReadAllText(FileName);
    }

    public bool IsGameTied()
    {
        var board = JsonSerializer.Deserialize<List<List<string>>>(GetSerializedBoardFileData())!;

        foreach (List<string> row in board)
        {
            if (row[0] == "" || row[1] == "" || row[2] == "")
            {
                return false;
            }
        }
        return true;
    }

    public List<List<int>>? GetWinningTiles()
    {
        var board = JsonSerializer.Deserialize<List<List<string>>>(GetSerializedBoardFileData())!;
        int[] y = [-1, -1, -1];
        int[] x = [-1, -1, -1];

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
