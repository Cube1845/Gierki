import { Injectable } from '@angular/core';
import {
  HttpTransportType,
  HubConnection,
  HubConnectionBuilder,
} from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { Move } from '../models/move';

@Injectable({
  providedIn: 'root',
})
export class BoardApiService {
  private apiUrl = 'https://localhost:7047/tictactoehub';
  private hubConnection!: HubConnection;

  private moveMadeSubject = new Subject<any>();
  moveMade$ = this.moveMadeSubject.asObservable();

  startConnection(): void {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.apiUrl, {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets,
      })
      .build();

    this.hubConnection.on('GameStarted', (data) => console.log(data));

    this.hubConnection.on('MoveMade', (data) =>
      this.moveMadeSubject.next(data)
    );

    this.hubConnection
      .start()
      .then(() => {
        console.log('Hub connected!');
      })
      .catch((err) => console.log('Error: ' + err));
  }

  startGame(): void {
    this.hubConnection?.invoke('StartGame');
  }

  makeMoveAndGetGameStatus(move: Move): void {
    this.hubConnection?.invoke('MakeMoveAndGetGameStatus', move);
  }
}
