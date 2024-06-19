
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Application.Persistence
{
    public class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
    {
        public DbSet<AppUsers> AppUsers { get; set; }
    }
}
