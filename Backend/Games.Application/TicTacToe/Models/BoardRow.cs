using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Application.TicTacToe.Models;
public record BoardRow(List<string> Positions)
{
    public static implicit operator BoardRow(List<string> Positions)
    {
        return new(Positions);
    }
}
