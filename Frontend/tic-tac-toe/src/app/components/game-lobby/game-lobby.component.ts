import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { LobbyApiService } from '../../services/lobby-api.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { User } from '../../models/user';
import { LobbyService } from '../../services/lobby.service';

@Component({
  selector: 'app-game-lobby',
  standalone: true,
  imports: [ ReactiveFormsModule ],
  templateUrl: './game-lobby.component.html',
  styleUrl: './game-lobby.component.scss'
})
export class GameLobbyComponent implements OnInit {
  constructor(private readonly lobbyApiService: LobbyApiService, private readonly lobbyService: LobbyService) {
    this.lobbyApiService.listUpdated$.pipe(takeUntilDestroyed()).subscribe((response) =>
      this.setWaitingUsers(response.value)
    );
  }

  username = new FormControl("", Validators.required);

  async ngOnInit(): Promise<void> {
    await this.lobbyApiService.startConnection();
    this.lobbyApiService.loadWaitingUsers();
  }

  @HostListener('window:unload', [ '$event' ])
  beforeUnloadHandler(event: any) {
    this.removeUserFromList(event)
    event.preventDefault();
    return false;   
  }

  unloadHandler(event: any) {
    this.removeUserFromList(event)
  }

  removeUserFromList(event: any){
    this.lobbyApiService.removeFromWaitingUsers();
  }

  joinButtonClick(): void {
    this.lobbyApiService.joinWaitingUsers(this.username.value!);

    var user = {
      'userId': this.lobbyApiService.getUserConnectionId(),
      'username': this.username.value!
    };
    this.lobbyService.setThisUser(user);

    this.username.reset();
    this.lobbyService.setThisUserWaiting(true);
  }

  stopWaitingButtonClick(): void {
    this.removeUserFromList(null);
    this.lobbyService.setThisUserWaiting(false);

    var emptyUser: User = {'userId': '', 'username': ''};
    this.lobbyService.setThisUser(emptyUser);
  }

  setWaitingUsers(users: User[]): void {
    if (users != null) {
      this.lobbyService.setWaitingUsers(users);
    }
  }

  getWaitingUsersCount(): number {
    return this.lobbyService.getWaitingUsersCount();
  }

  getWaitingUsernames(): string[] {
    return this.lobbyService.getWaitingUsernames();
  }

  isUserWaiting(): boolean {
    return this.lobbyService.isThisUserWaiting();
  }
}
