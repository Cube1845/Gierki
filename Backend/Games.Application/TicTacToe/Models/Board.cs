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
}
