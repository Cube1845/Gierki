using Games.Application.Infrastructure;
using Games.Application.TicTacToe.Models;
using Games.Application.TicTacToe.Services;
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

        public async Task MakeMoveAndGetGameData(Move move)
        {
            await Clients.All.SendAsync("MoveMade", await _ticTacToeService.MakeMoveAndGetGameData(move));
        }

        public async Task LoadGameData() 
        {
            await Clients.All.SendAsync("DataLoaded", await _ticTacToeService.LoadGameData(Context.ConnectionId));
        }

    }
}
