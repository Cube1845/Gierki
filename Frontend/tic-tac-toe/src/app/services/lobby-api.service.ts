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

  private gameStartedSubject = new Subject<any>();
  gameStarted$ = this.gameStartedSubject.asObservable();

  private getUserDataSubject = new Subject<any>();
  getUserData$ = this.getUserDataSubject.asObservable();

  async startConnection(): Promise<void> {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.apiUrl, {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets,
        accessTokenFactory: () => localStorage.getItem("token")!
      })
      .build();

    this.hubConnection.on("WaitingListUpdated", (response) => {
      this.ListUpdatedSubject.next(response);
    });

    this.hubConnection.on("StartGame", (response) => {
      this.gameStartedSubject.next(response);
    });

    this.hubConnection.on("GetThisUserData", (response) => {
      this.getUserDataSubject.next(response);
    });

    await this.hubConnection
      .start()
      .then(() => {
        console.log('Hub connected!');
      })
      .catch((err) => console.log('Error: ' + err));

    this.hubConnection.invoke("GetThisUserData");
  }

  removeFromWaitingUsers(): void {
    this.hubConnection.invoke("RemoveUserFromWaitingPlayers");
  }

  joinWaitingUsers(): void {
    this.hubConnection.invoke("AddUserToWaitingPlayers");
  }

  loadWaitingUsers(): void {
    this.hubConnection.invoke("GetWaitingList");
  }

  stopConnection(): void {
    this.hubConnection.stop();
  }
}
