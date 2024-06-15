import { Injectable } from '@angular/core';
import {
  HttpTransportType,
  HubConnection,
  HubConnectionBuilder,
} from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { Move } from '../models/move';
import { Position } from '../models/position';

@Injectable({
  providedIn: 'root',
})
export class BoardApiService {
  private apiUrl = 'https://localhost:7047/tictactoehub';
  private hubConnection!: HubConnection;

  private moveMadeSubject = new Subject<any>();
  moveMade$ = this.moveMadeSubject.asObservable();

  private dataLoadedSubject = new Subject<any>();
  dataLoaded$ = this.dataLoadedSubject.asObservable();

  async startConnection(): Promise<void> {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.apiUrl, {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets,
      })
      .build();

    this.hubConnection.on('MoveMade', (data) =>
      this.moveMadeSubject.next(data)
    );

    this.hubConnection.on('DataLoaded', (data) =>
      this.dataLoadedSubject.next(data)
    );

    await this.hubConnection
      .start()
      .then(() => {
        console.log('Hub connected!');
      })
      .catch((err) => console.log('Error: ' + err));
  }

  makeMoveAndGetGameStatus(move: Move): void {
    this.hubConnection.invoke('MakeMoveAndGetGameData', move);
  }

  loadGameData(): void {
    this.hubConnection.invoke('LoadGameData');
  }
}
