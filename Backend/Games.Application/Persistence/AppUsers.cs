using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Application.Persistence
{
    public class AppUsers
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string? Email { get; set; }
        public string Password { get; set; } = "";
    }
}
