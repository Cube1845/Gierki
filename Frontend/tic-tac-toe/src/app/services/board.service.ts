import { Injectable } from '@angular/core';
import { BoardApiService } from './board-api.service';
import { GameData } from '../models/gameData';
import { Position } from '../models/position';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root',
})
export class BoardService {
  gameData: GameData = new GameData();
  thisUser: User = {username: '', guid: ''};

  constructor(private readonly boardApiService: BoardApiService) {}

  setGameData(data: GameData): void {
    if (data != null) {
      this.gameData = data;
    }
  }

  makeMoveAndGetGameStatus(pos: Position): void {
    var move = {
      symbol: this.gameData.turn,
      position: pos
    };
    this.boardApiService.makeMoveAndGetGameStatus(move);
  }

  getThisUsersTurn(): string {
    var symbol = "";

    this.gameData.players.forEach(userRole => {
      if (userRole.user.username == this.thisUser.username) {
        symbol = userRole.symbol;
      }
    });

    return symbol;
  }

  setThisUser(user: User): void {
    this.thisUser = user;
  }

  getWinner(): string | null {
    return this.gameData.gameWinnedBy;
  }

  isGameTied(): boolean {
    return this.gameData.isGameTied;
  }

  getGameState(): boolean {
    return this.gameData.isGameStarted;
  }

  getTurn(): string {
    return this.gameData.turn;
  }
}
