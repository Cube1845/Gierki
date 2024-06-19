using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Application.Authentication.Models
{
    public record LoginUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
