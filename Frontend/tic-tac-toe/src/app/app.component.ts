import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { BoardComponent } from './components/board/board.component';
import { NgIf } from '@angular/common';
import { BoardApiService } from './services/board-api.service';
import { tap } from 'rxjs';
import { BoardService } from './services/board.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, BoardComponent, NgIf],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  constructor(private readonly boardApiService: BoardApiService, private readonly boardService: BoardService) {}

  async ngOnInit(): Promise<void> {
    await this.boardApiService.startConnection();
    this.boardApiService.loadGameData();
  }

  startGame(): void {
    this.boardApiService.startGame();
  }

  getWinner(): string | null{
    if (this.boardService.getWinner() != null) {
      return this.boardService.getWinner();
    } else {
      return "";
    }
  }

  isGameWinned(): boolean {
    return this.boardService.getWinner() != null;
  }

  isGameTied(): boolean {
    return this.boardService.isGameTied();
  }

  getGameState(): boolean {
    return this.boardService.getGameState();
  }

  getTurn(): string {
    return this.boardService.getTurn();
  }
}