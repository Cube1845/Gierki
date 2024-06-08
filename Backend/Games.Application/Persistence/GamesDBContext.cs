using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Application.Persistence
{
    public class GamesDbContext(DbContextOptions<GamesDbContext> options) : DbContext(options)
    {
        public DbSet<TicTacToe> TicTacToe { get; set; }
    }
}
