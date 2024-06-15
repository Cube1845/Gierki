using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Application.TicTacToe.Models
{
    public class UserRole(User user, string symbol)
    {
        public User User { get; set; } = user;
        public string Symbol { get; set; } = symbol;
    }
}
