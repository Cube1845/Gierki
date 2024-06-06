import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class BoardService {
  isGameStarted: boolean = false;
  gameWinnedBy: string | null = null;
  gameTied: boolean = false;
  turn: string = "";
  board: string[][] = [["","",""],["","",""],["","",""]];

  isGameTied(): boolean {
    return this.gameTied;
  }

  getWinner(): string | null {
    return this.gameWinnedBy;
  }

  resetStateVariables(): void {
    this.gameTied = false;
    this.gameWinnedBy = null;
  }

  gameWin(symbol: string): void {
    this.gameWinnedBy = symbol;
    this.setGameState(false);
    this.setTurn("");
  }

  gameTie(): void {
    this.gameTied = true;
    this.setGameState(false);
    this.setTurn("");
  }

  updateBoard(board: string[][]): void {
    this.board = board;
  }

  getBoard(): string[][] {
    return this.board;
  }

  clearBoard(): void {
    for (let i = 0; i < 3; i++) {
      for (let j = 0; j < 3; j++) {
        this.board[i][j] = "";
      }
    }
  }

  getTurn(): string {
    return this.turn;
  }

  setTurn(symbol: string): void {
    this.turn = symbol;
  }

  changeTurn(): void {
    if (this.getTurn() == "O") {
      this.setTurn("X");
    } else {
      this.setTurn("O");
    }
  }

  setGameState(bool: boolean): void {
    this.isGameStarted = bool;
  }

  getGameState(): boolean {
    return this.isGameStarted;
  }
}

