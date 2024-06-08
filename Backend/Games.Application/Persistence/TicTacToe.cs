using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Application.Persistence;
public class TicTacToe
{
    [Key]
    public int Id { get; set; }
    public string Board { get; set; } = "";
    public bool IsGameStarted { get; set; }
    public string? GameWinnedBy { get; set; }
    public bool IsGameTied { get; set; }
    public string Turn { get; set; } = "";
}
