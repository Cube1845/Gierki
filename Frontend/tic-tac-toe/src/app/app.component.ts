import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { BoardComponent } from './components/board/board.component';
import { NgIf } from '@angular/common';
import { BoardApiService } from './services/board-api.service';
import { tap } from 'rxjs';
import { BoardService } from './services/board.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, BoardComponent, NgIf],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  constructor(private readonly boardApiService: BoardApiService, private readonly boardService: BoardService) {}

  responseText: string = "";

  startGame(): void {
    this.boardService.setGameState(true);
    this.boardApiService.startGame().subscribe((text) => this.responseText = text);
    this.boardService.clearBoard();
    this.boardService.setTurn("O");
    this.boardService.updateBoard([["","",""],["","",""],["","",""]]);
    this.boardService.resetStateVariables();
  }

  getGameState(): boolean {
    return this.boardService.getGameState();
  }

  getTurn(): string {
    return this.boardService.getTurn();
  }

  isGameWinned(): boolean {
    return (this.boardService.getWinner() != null)
  }

  isGameTied(): boolean {
    return this.boardService.isGameTied();
  }

  getWinner(): string | null {
    return this.boardService.getWinner();
  }
}