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
    public class LobbyService(GamesDbContext context, TicTacToeService ticTacToe)
    {
        private readonly GamesDbContext _context = context;

        private readonly TicTacToeService _ticTacToe = ticTacToe;
        
        public async Task<Result<List<User>>> GetWaitingList()
        {
            return Result<List<User>>.Success(await GetUsersFromDatabase(), "List sent");
        }

        public async Task<Result<List<User>>> AddPlayerToWaitingList(string username, string connectionId)
        {
            var waitingPlayers = await GetUsersFromDatabase();
            if (waitingPlayers.Any(p => p.ConnectionId == connectionId))
            {
                return Result<List<User>>.Error("Client already in");
            }

            var user = new Persistence.Users()
            {
                Name = username,
                UserId = connectionId
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            waitingPlayers = await GetUsersFromDatabase();
            if (waitingPlayers.Count < 2)
            {
                return Result<List<User>>.Success(await GetUsersFromDatabase(), "Added user to waiting list");
            }
            
            await RemovePlayerFromWaitingList(waitingPlayers[0].ConnectionId);
            await RemovePlayerFromWaitingList(waitingPlayers[1].ConnectionId);

            await _ticTacToe.StartGame(new List<User>() { waitingPlayers[0], waitingPlayers[1] });

            return Result<List<User>>.Success(waitingPlayers, "Start game");
        }

        public async Task<Result<List<User>>> RemovePlayerFromWaitingList(string connectionId)
        {
            var usersDb = await GetUsersFromDatabase();
            if (usersDb == null)
            {
                return Result<List<User>>.Error("List empty");
            }

            var user = await _context.Users.FindAsync(connectionId);
            if (user == null)
            {
                return Result<List<User>>.Error("User not found");
            }
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
                    ConnectionId = user.UserId
                });
            }

            return users;
        }
    }
}
