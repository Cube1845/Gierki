using Games.Application.TicTacToe.Services;
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

        public async Task GetConnectionId()
        {
            await Clients.Caller.SendAsync("GetConnectionId", Context.ConnectionId);
        }

        public async Task JoinToWaitingPlayers(string username)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, _waitingPlayers);

            var response = await _lobbyService.AddPlayerToWaitingList(username, Context.ConnectionId);
            if (response.Message == "Start game")
            {
                await Clients.Group(_waitingPlayers).SendAsync("StartGame", response);
            }
            else
            {
                await Clients.All.SendAsync("WaitingListUpdated", response);
            }
        }

        public async Task RemovePlayerFromWaitingList(string userId, bool useParameter)
        {
            if (useParameter)
            {
                await Groups.RemoveFromGroupAsync(userId, _waitingPlayers);

                var response = await _lobbyService.RemovePlayerFromWaitingList(userId);
                await Clients.All.SendAsync("WaitingListUpdated", response);
            }
            else
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, _waitingPlayers);

                var response = await _lobbyService.RemovePlayerFromWaitingList(Context.ConnectionId);
                await Clients.All.SendAsync("WaitingListUpdated", response);
            }
        }

        public async Task GetWaitingList()
        {
            var response = await _lobbyService.GetWaitingList();
            await Clients.Caller.SendAsync("WaitingListUpdated", response);
        }
    }
}
