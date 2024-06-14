using Games.Application.Infrastructure;
using Games.Application.Persistence;
using Games.Application.TicTacToe.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Application.TicTacToe.Services
{
    public class LobbyService(GamesDbContext context)
    {
        private readonly GamesDbContext _context = context;
         
        public async Task<Result<List<User>>> AddPlayerToWaitingList(string username, string connectionId)
        {
            var usersDb = await _context.Users.FirstOrDefaultAsync();

            var user = new Persistence.Users()
            {
                Name = username,
                UserId = connectionId
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var waitingPlayers = await GetUsersFromDatabase();
            if (waitingPlayers.Any(p => p.UserId != connectionId))
            {
                return Result<List<User>>.Success(waitingPlayers, "Added user to waiting list");
            }

            return Result<List<User>>.Error("Client already in");
        }

        public async Task<Result<List<User>>> RemovePlayerFromWaitingList(string connectionId)
        {
            var waitingPlayers = await GetUsersFromDatabase();
            if (waitingPlayers.Count < 1)
            {
                return Result<List<User>>.Error("There are no waiting users");
            }

            var user = await _context.Users.FindAsync(connectionId);
            _context.Users.Remove(user!);
            await _context.SaveChangesAsync();

            return Result<List<User>>.Success(await GetUsersFromDatabase(), "Successfully removed user from list");
        }

        private async Task<List<User>> GetUsersFromDatabase()
        {
            var usersDb = await _context.Users.FirstOrDefaultAsync();

            if (usersDb == null)
            {
                return new List<User>();
            }

            var usersFromDb = await _context.Users.ToListAsync();
            var users = new List<User>();
            foreach (var user in usersFromDb)
            {
                users.Add(new User()
                {
                    Username = user.Name,
                    UserId = user.UserId
                });
            }

            return users;
        }
    }
}
