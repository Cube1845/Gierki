import { Component, OnInit } from '@angular/core';
import { BoardApiService } from '../../services/board-api.service';
import { BoardService } from '../../services/board.service';
import { NgIf } from '@angular/common';
import { BoardComponent } from '../board/board.component';

@Component({
  selector: 'app-game-menu',
  standalone: true,
  imports: [ NgIf, BoardComponent ],
  templateUrl: './game-menu.component.html',
  styleUrl: './game-menu.component.scss'
})

export class GameMenuComponent implements OnInit {
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
