import { HttpClient } from '@angular/common/http';
import { Injectable, OnInit } from '@angular/core';
import { Observable, map } from 'rxjs';
import { Move } from '../models/move';
import { Result } from '../models/result';
import { BoardService } from './board.service';
import { HttpTransportType, HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

@Injectable({
  providedIn: 'root',
})
export class BoardApiService{
  apiUrl = 'https://localhost:7047/tictactoehub';

  hubConnection: HubConnection | undefined;

  startConnection(): void {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.apiUrl, {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets
      })
      .build();

    this.hubConnection.on("GameStarted", (data) => console.log(data));

    this.hubConnection.on("MoveMade", (data) => console.log(data));

    this.hubConnection
      .start()
      .then(() => {
        console.log("Hub connected!");
      })
      .catch((err) => console.log("Error: " + err));
  }

  startGame(): void {
    this.hubConnection?.invoke("StartGame");
  } 

  makeMoveAndGetGameStatus(move: Move): void {
    var text = JSON.stringify(move);
    this.hubConnection?.invoke("MakeMoveAndGetGameData", text);
  } 
}
