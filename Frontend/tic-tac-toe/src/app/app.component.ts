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
  constructor(private readonly boardApiService: BoardApiService, private readonly boardService: BoardService) {
    this.boardApiService.moveMade$?.pipe(takeUntilDestroyed()).subscribe((data) => 
      this.boardService.setGameData(data.value)
    );
    this.boardApiService.gameStarted$?.pipe(takeUntilDestroyed()).subscribe((data) =>
      this.boardService.setGameData(data.value)
    );
  }

  ngOnInit(): void {
    this.boardApiService.startConnection();
  }

  startGame(): void {
    this.boardApiService.startGame();
  }

  // getGameState(): boolean {
  //   return this.boardService.getGameState();
  // }

  // getTurn(): string {
  //   return this.boardService.getTurn();
  // }
}