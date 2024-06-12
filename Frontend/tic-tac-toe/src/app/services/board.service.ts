import { Injectable } from '@angular/core';
import { BoardApiService } from './board-api.service';
import { GameData } from '../models/gameData';
import { Position } from '../models/position';
import { Board } from '../models/board';

@Injectable({
  providedIn: 'root',
})
export class BoardService {
  board: Board = {"boardData":[{"positions": ["", "", ""]}, {"positions": ["", "", ""]}, {"positions": ["", "", ""]}]};
  isGameStarted: boolean = false;
  gameWinnedBy: string | null = null;
  isGameTied: boolean = false;
  turn: string = "O";
  winningTiles: Position[] | null = null;

  constructor(private readonly boardApiService: BoardApiService) {}

  setGameData(data: GameData): void {
    this.board = data.board;
    this.isGameStarted = data.isGameStarted;
    this.gameWinnedBy = data.gameWinnedBy;
    this.isGameTied = data.isGameTied;
    this.turn = data.turn;
    this.winningTiles = data.winningTiles;
  }

  makeMoveAndGetGameStatus(pos: Position): void {
    this.boardApiService.makeMoveAndGetGameStatus({
      "symbol": this.turn,
      "position": pos
    });
  }

  getBoardTile(x: number, y: number): string {
    return this.board.boardData[y].positions[x];
  }
}
