﻿using Games.Application.TicTacToe.Services;
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

        public async Task JoinToWaitingPlayers(string username)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, _waitingPlayers);
            var response = await _lobbyService.AddPlayerToWaitingList(username, Context.ConnectionId);
            await Clients.Group(_waitingPlayers).SendAsync("WaitingListUpdated", response);
        }

        public async Task RemovePlayerFromWaitingList()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, _waitingPlayers);
            var response = await _lobbyService.RemovePlayerFromWaitingList(Context.ConnectionId);
            await Clients.Group(_waitingPlayers).SendAsync("WaitingListUpdated", response);
        }
    }
}
