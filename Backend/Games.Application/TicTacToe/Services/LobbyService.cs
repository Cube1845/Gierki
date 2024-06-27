using Games.Application.Infrastructure;
using Games.Application.Persistence;
using Games.Application.TicTacToe.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Games.Application.TicTacToe.Services
{
    public class LobbyService(GamesDbContext context, TicTacToeService ticTacToe)
    {
        private readonly GamesDbContext _context = context;

        private readonly TicTacToeService _ticTacToe = ticTacToe;
        
        public Result<List<User>> GetWaitingUsers()
        {
            return Result<List<User>>.Success(WaitingUsers.Instance.GetUsers(), "");
        }

        public async Task<Result<List<User>>> AddUserToWaitingPlayers(User user)
        {
            if (WaitingUsers.Instance.GetUsers().Contains(user))
            {
                return Result<List<User>>.Error("This user is already waiting");
            }

            WaitingUsers.Instance.AddUser(user);

            if (WaitingUsers.Instance.GetUsers().Count > 1)
            {
                var users = WaitingUsers.Instance.GetUsers();
                WaitingUsers.Instance.ClearUsers();

                await _ticTacToe.StartGame(users);

                return Result<List<User>>.Success(users, "Start game");
            }

            return Result<List<User>>.Success(WaitingUsers.Instance.GetUsers(), "User added to waiting players");
        }

        public Result<List<User>> RemoveUserFromWaitingPlayers(User user)
        {
            WaitingUsers.Instance.RemoveUser(user);
            return Result<List<User>>.Success(WaitingUsers.Instance.GetUsers(), "User removed from waiting players");
        }
    }
}
