import { Component } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Move } from '../../models/move';
import { BoardApiService } from '../../services/board-api.service';
import { BoardService } from '../../services/board.service';
import { TileComponent } from '../tile/tile.component';
import { Position } from '../../models/position';
import { stringify } from 'querystring';
import { Board } from '../../models/board';
import { GameData } from '../../models/gameData';

@Component({
  selector: 'app-board',
  standalone: true,
  imports: [TileComponent],
  templateUrl: './board.component.html',
  styleUrl: './board.component.scss',
})
export class BoardComponent {
  board: Board = new GameData().board;

  constructor(
    private readonly boardService: BoardService,
    private readonly boardApiService: BoardApiService
  ) {
    this.boardApiService.moveMade$?.pipe(takeUntilDestroyed()).subscribe((data) => 
      this.updateGameData(data.value)
    );

    this.boardApiService.gameStarted$?.pipe(takeUntilDestroyed()).subscribe((data) =>
      this.updateGameData(data.value)
    );

    this.boardApiService.dataLoaded$?.pipe(takeUntilDestroyed()).subscribe((data) =>
      this.updateGameData(data.value)
    );
  }

  updateGameData(data: GameData | null): void {
    if (data != null) {
      this.board = (data.board); 
      this.boardService.setGameData(data);
    }
  }

  onTileClick(pos: Position): void {
    this.boardService.makeMoveAndGetGameStatus(pos);
  }
}
