import { Injectable } from '@angular/core';
import { HttpTransportType, HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LobbyApiService {
  private apiUrl = 'https://localhost:7047/lobbyhub';
  private hubConnection!: HubConnection;

  private ListUpdatedSubject = new Subject<any>();
  listUpdated$ = this.ListUpdatedSubject.asObservable();

  async startConnection(): Promise<void> {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.apiUrl, {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets,
      })
      .build();

    this.hubConnection.on("WaitingListUpdated", (users) => {
      this.ListUpdatedSubject.next(users);
    });

    await this.hubConnection
      .start()
      .then(() => {
        console.log('Hub connected!');
      })
      .catch((err) => console.log('Error: ' + err));
  }

  stopConnection(): void {
    this.hubConnection.stop();
  }

  removeFromWaitingUsers(): void {
    this.hubConnection.invoke("RemovePlayerFromWaitingList");
  }

  joinWaitingUsers(username: string): void {
    this.hubConnection.invoke("JoinToWaitingPlayers", username);
  }

  loadWaitingUsers(): void {
    this.hubConnection.invoke("GetWaitingList");
  }

  getUserConnectionId(): string {
    return this.hubConnection.connectionId!;
  }
}
