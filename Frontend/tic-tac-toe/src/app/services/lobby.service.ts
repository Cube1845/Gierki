import { Injectable } from '@angular/core';
import { LobbyApiService } from './lobby-api.service';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class LobbyService {
  waitingPlayers: User[] | null = null;

  getWaitingPlayers(): User[] | null {
    return this.waitingPlayers;
  }

  setWaitingPlayers(users: User[] | null): void {
    this.waitingPlayers = users;
  }
}
