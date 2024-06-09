using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Application.TicTacToe.Models;
public record Board(List<BoardRow> BoardData)
{
    public static implicit operator Board(List<BoardRow> BoardData)
    {
        return new(BoardData); 
    }

    public static Board ConvertFromMultiListToBoardModel(List<List<string>> listedBoard)
    {
        List<BoardRow> boardRows = new();
        BoardRow boardRow = new List<string>();

        for (int i = 0; i < listedBoard.Count; i++)
        {
            boardRow = new List<string>();

            for (int j = 0; j < listedBoard[i].Count; j++)
            {
                boardRow.Positions.Add(listedBoard[i][j]);
            }

            boardRows.Add(boardRow);
        }

        return new Board(boardRows);
    }

    public static List<List<string>> ConvertFromBoardModelToMultiList(Board board)
    {
        var listedBoard = new List<List<string>>();
        var listedBoardRow = new List<string>();

        for (int i = 0; i < board.BoardData.Count; i++)
        {
            listedBoardRow = new List<string>();

            for (int j = 0; j < board.BoardData[i].Positions.Count; j++)
            {
                listedBoardRow.Add(board.BoardData[i].Positions[j]);
            }

            listedBoard.Add(listedBoardRow);
        }

        return listedBoard;
    }
}
