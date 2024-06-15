import { Injectable } from '@angular/core';
import { HttpTransportType, HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LobbyApiService {
  private apiUrl = 'https://localhost:7047/lobbyhub';
  private hubConnection!: HubConnection;

  private connectionId!: string;

  private ListUpdatedSubject = new Subject<any>();
  listUpdated$ = this.ListUpdatedSubject.asObservable();

  private gameStartedSubject = new Subject<any>();
  gameStarted$ = this.gameStartedSubject.asObservable();

  async startConnection(): Promise<void> {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.apiUrl, {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets,
      })
      .build();

    this.hubConnection.on("GetConnectionId", (response) => {
      this.connectionId = response;
    });
    
    this.hubConnection.on("WaitingListUpdated", (response) => {
      this.ListUpdatedSubject.next(response);
    });

    this.hubConnection.on("StartGame", (response) => {
      this.gameStartedSubject.next(response);
    });

    await this.hubConnection
      .start()
      .then(() => {
        console.log('Hub connected!');
      })
      .catch((err) => console.log('Error: ' + err));

    this.hubConnection.invoke("GetConnectionId");
  }

  stopConnection(): void {
    this.hubConnection.stop();
  }

  removeFromWaitingUsers(connectionId: string | null): void {
    if (connectionId == null) {
      this.hubConnection.invoke("RemovePlayerFromWaitingList", null, false);
    } else {
      this.hubConnection.invoke("RemovePlayerFromWaitingList", connectionId, true);
    }
  }

  joinWaitingUsers(username: string): void {
    this.hubConnection.invoke("JoinToWaitingPlayers", username);
  }

  loadWaitingUsers(): void {
    this.hubConnection.invoke("GetWaitingList");
  }

  getUserConnectionId(): string {
    return this.connectionId;
  }
}
