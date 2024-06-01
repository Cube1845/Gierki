import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { BoardComponent } from './components/board/board.component';
import { BoardService } from './services/board.service';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, BoardComponent, NgIf],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  constructor(private readonly boardService: BoardService) {}

  startGame(): void {
    this.boardService.clearBoard();
    this.boardService.setGameStatus(true);
    this.boardService.turn = "O";
  }

  getTurn(): string {
    return this.boardService.getTurn();
  }

  getGameStatus(): boolean {
    return this.boardService.getGameStatus();
  }

  isGameWinned(): boolean {
    if (this.boardService.getWinningTiles() == null) {
      return false;
    }
    return true;
  }

  isGameTied(): boolean {
    return this.boardService.isGameTied();
  }
}
