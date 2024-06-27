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
    public class LobbyHub(LobbyService lobbyService) : Hub
    {
        private readonly LobbyService _lobbyService = lobbyService;

        private readonly string _waitingPlayers = "WaitingPlayers";

        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            User user = new User()
            {
                Username = Context.User.Claims.ToList()[2].Value,
                GUID = Context.User.Claims.ToList()[1].Value
            };

            _lobbyService.RemoveUserFromWaitingPlayers(user);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, _waitingPlayers);
        }

        [Authorize]
        public async Task AddUserToWaitingPlayers()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, _waitingPlayers);

            User user = new User() 
            { 
                Username = Context.User.Claims.ToList()[2].Value,
                GUID = Context.User.Claims.ToList()[1].Value 
            };

            var response = await _lobbyService.AddUserToWaitingPlayers(user);

            if (!response.IsSuccess)
            {
                await Clients.Caller.SendAsync("Error", response.Message);
                return;
            }

            if (response.Message == "Start game")
            {
                await Clients.Group(_waitingPlayers).SendAsync("StartGame", response);
            }
            else
            {
                await Clients.All.SendAsync("WaitingListUpdated", response);
            }
        }

        [Authorize]
        public async Task GetWaitingList()
        {
            var response = _lobbyService.GetWaitingUsers();
            await Clients.Caller.SendAsync("WaitingListUpdated", response);
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

        [Authorize]
        public async Task RemoveUserFromWaitingPlayers()
        {
            User user = new User()
            {
                Username = Context.User.Claims.ToList()[2].Value,
                GUID = Context.User.Claims.ToList()[1].Value
            };

            var response = _lobbyService.RemoveUserFromWaitingPlayers(user);
            await Clients.All.SendAsync("WaitingListUpdated", response);
        }
    }
}
