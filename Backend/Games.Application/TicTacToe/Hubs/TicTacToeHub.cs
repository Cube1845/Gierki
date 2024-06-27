using Games.Application.Infrastructure;
using Games.Application.Persistence;
using Games.Application.TicTacToe.Models;
using Games.Application.TicTacToe.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Application.TicTacToe.Hubs
{
    public class TicTacToeHub(TicTacToeService ticTacToeService) : Hub
    {
        private readonly TicTacToeService _ticTacToeService = ticTacToeService;

        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            User user = new User()
            {
                Username = Context.User.Claims.ToList()[2].Value,
                GUID = Context.User.Claims.ToList()[1].Value
            };

            await _ticTacToeService.RemoveGameDataFromDatabase(user);
        }

        [Authorize]
        public async Task MakeMoveAndGetGameData(Move move)
        {
            User user = new User()
            {
                Username = Context.User.Claims.ToList()[2].Value,
                GUID = Context.User.Claims.ToList()[1].Value
            };

            await Clients.All.SendAsync(
                "MoveMade",
                await _ticTacToeService.MakeMoveAndGetGameData(move, user));
        }

        [Authorize]
        public async Task LoadGameData() 
        {
            User user = new User()
            {
                Username = Context.User.Claims.ToList()[2].Value,
                GUID = Context.User.Claims.ToList()[1].Value
            };

            await Clients.Caller.SendAsync(
                "DataLoaded",
                await _ticTacToeService.LoadGameData(user));
        }

        [Authorize]
        public async Task GetThisUserData()
        {
            User user = new User()
            {
                Username = Context.User.Claims.ToList()[2].Value,
                GUID = Context.User.Claims.ToList()[1].Value
            };

            await Clients.Caller.SendAsync("GetThisUserData", user);
        }
    }
}
