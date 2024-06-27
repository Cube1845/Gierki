import { Component, HostListener, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { LobbyApiService } from '../../services/lobby-api.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { User } from '../../models/user';
import { LobbyService } from '../../services/lobby.service';
import { Router } from '@angular/router';
import { BoardApiService } from '../../services/board-api.service';
import { AuthApiService } from '../../services/auth-api.service';
import { HttpErrorResponse } from '@angular/common/http';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-game-lobby',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './game-lobby.component.html',
  styleUrl: './game-lobby.component.scss',
})
export class GameLobbyComponent implements OnInit {
  constructor(
    private readonly lobbyApiService: LobbyApiService,
    private readonly lobbyService: LobbyService,
    private readonly router: Router,
    private readonly authApiService: AuthApiService,
    @Inject(DOCUMENT) private document: Document
  ) {
    this.lobbyApiService.listUpdated$
      .pipe(takeUntilDestroyed())
      .subscribe((response) => this.setWaitingUsers(response.value));

    this.lobbyApiService.gameStarted$
      .pipe(takeUntilDestroyed())
      .subscribe((response) => this.startGame(response.value));

    this.lobbyApiService.getUserData$
      .pipe(takeUntilDestroyed())
      .subscribe((response) => this.setThisUser(response));
  }

  async ngOnInit(): Promise<void> {
    const localStorage = this.document.defaultView?.localStorage;

    if (!localStorage) {
      return;
    }

    const token = localStorage.getItem('token')!;
    
    this.authApiService.isUserAuthenticated(token).subscribe(
      async (value) => {
        if (value) {
          await this.loadComponentValues();
        }
      },
      (err: HttpErrorResponse) => {
        if (err.status == 401) {
          this.router.navigateByUrl('login');
        }
      }
    );
  }

  async loadComponentValues(): Promise<void> {
    await this.lobbyApiService.startConnection();
    this.lobbyApiService.loadWaitingUsers();
    this.lobbyService.setThisUserWaiting(false);
  }

  @HostListener('window:unload', ['$event'])
  beforeUnloadHandler(event: any) {
    this.removeUserFromList(event);
    event.preventDefault();
    return false;
  }

  unloadHandler(event: any) {
    this.removeUserFromList(event);
  }

  removeUserFromList(event: any): void {
    this.lobbyApiService.removeFromWaitingUsers();
  }

  setThisUser(user: User): void {
    this.lobbyService.setThisUser(user);
  }

  startGame(users: User[]): void {
    var isGameCorrect: boolean = false;

    users.forEach(user => {
      if (user.username == this.lobbyService.getThisUser().username) {
        isGameCorrect = true;
      }
    });

    if (isGameCorrect) {
      this.lobbyApiService.stopConnection();
      this.router.navigateByUrl('game');
    }
  }

  joinButtonClick(): void {
    this.lobbyApiService.joinWaitingUsers();
    this.lobbyService.setThisUserWaiting(true);
  }

  stopWaitingButtonClick(): void {
    this.removeUserFromList(null);
    this.lobbyService.setThisUserWaiting(false);
  }

  setWaitingUsers(users: User[]): void {
    console.log(users);
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
