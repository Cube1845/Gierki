import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class BoardService {

  winningCases: any  = [[[0, 0], [0, 1], [0, 2]],
                        [[1, 0], [1, 1], [1, 2]],
                        [[2, 0], [2, 1], [2, 2]],
                        [[0, 0], [1, 0], [2, 0]],
                        [[0, 1], [1, 1], [2, 1]],
                        [[0, 2], [1, 2], [2, 2]],
                        [[0, 0], [1, 1], [2, 2]],
                        [[0, 2], [1, 1], [2, 0]],];
  isGameStarted: boolean = false;
  turn: string = "";
  board: string[][] = [["", "", ""], 
                       ["", "", ""], 
                       ["", "", ""]]

  getGameStatus(): boolean {
    return this.isGameStarted;
  }

  setGameStatus(boolean: boolean): void {
    this.isGameStarted = boolean;
  }

  getBoard(): string[][] {
    return this.board;
  }
                              
  getTurn(): string {
    return this.turn;
  }

  clearBoard(): void {
    for (let i = 0; i < 3; i++) {
      for (let j = 0; j < 3; j++) {
        this.board[i][j] = "";
      }
    }
  }

  changeTurn(): void {
    if (this.getTurn() == "O") {
      this.turn = "X";
    } else {
      this.turn = "O";
    }
  }

  endGame(winningTilesNumber: number): void {
    this.clearBoard();
    for (let i = 0; i < 3; i++) {
      this.board[this.winningCases[winningTilesNumber][i][0]][this.winningCases[winningTilesNumber][i][1]] = this.turn;
    }
    this.setGameStatus(false);
  }

  isGameTied(): boolean {
    for (let i = 0; i < 3; i++) {
      for (let j = 0; j < 3; j++) {
        if (this.board[i][j] == "") {
          return false;
        }
      }
    }
    return true;
  }

  tie(): void {
    this.setGameStatus(false);
  }

  makeMove(tile: number[]): void {
    this.board[tile[0]][tile[1]] = this.turn;
    if (this.getWinningTiles() != null) {
      this.endGame(this.getWinningTiles()!);
    } else if (this.isGameTied()) {
      this.tie();
    } else {
      this.changeTurn();
    }
  } 

  getWinningTiles(): number | null {
    for (let i = 0; i < 8; i++) {
      if (this.board[this.winningCases[i][0][0]][this.winningCases[i][0][1]] == this.board[this.winningCases[i][1][0]][this.winningCases[i][1][1]] &&
          this.board[this.winningCases[i][1][0]][this.winningCases[i][1][1]] == this.board[this.winningCases[i][2][0]][this.winningCases[i][2][1]] && 
          this.board[this.winningCases[i][0][0]][this.winningCases[i][0][1]] != "") {
            return i;
          }
    }
    return null;
  }
}

