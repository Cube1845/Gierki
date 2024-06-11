using Games.Application.Infrastructure;
using Games.Application.TicTacToe.Models;
using Games.Application.TicTacToe.Services;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Games.Application.TicTacToe.Hubs
{
    public class TicTacToeHub(TicTacToeService ticTacToeService) : Hub
    {
        private readonly TicTacToeService _ticTacToeService = ticTacToeService;

        public async Task StartGame() 
        {
            await _ticTacToeService.StartGame();
            await Clients.All.SendAsync("GameStarted", Result.Success("Game started"));
        }

        public async Task MakeMoveAndGetGameStatus(Move move)
        {
            var serializedMove = JsonConvert.SerializeObject(move);
            var response = await _ticTacToeService.DeserializeStringAndMakeMove(serializedMove);
            await Clients.All.SendAsync("MoveMade", response);
        }
    }
}
