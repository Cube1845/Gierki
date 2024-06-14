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
    this.lobbyApiService.listUpdated$.pipe(takeUntilDestroyed()).subscribe((users) =>
      this.setWaitingPlayers(users.value)
    );
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
    this.lobbyApiService.removeFromWaitingPlayers();
  }

  ngOnInit(): void {
    this.lobbyApiService.startConnection();
  }

  username = new FormControl("", Validators.required);

  joinButtonClick(): void {
    this.lobbyApiService.joinWaitingPlayers(this.username.value!);
  }

  setWaitingPlayers(users: User[] | null): void {
    //make this work
    this.lobbyService.setWaitingPlayers(users);
  }
}
