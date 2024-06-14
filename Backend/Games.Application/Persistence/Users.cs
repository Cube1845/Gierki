using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Application.Persistence
{
    public class Users
    {
        public string Name { get; set; } = "";
        [Key]
        public string UserId { get; set; } = "";
    }
}
