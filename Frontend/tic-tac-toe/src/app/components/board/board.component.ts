import { Component } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Move } from '../../models/move';
import { BoardApiService } from '../../services/board-api.service';
import { BoardService } from '../../services/board.service';
import { TileComponent } from '../tile/tile.component';
import { Position } from '../../models/position';
import { stringify } from 'querystring';
import { Board } from '../../models/board';

@Component({
  selector: 'app-board',
  standalone: true,
  imports: [TileComponent],
  templateUrl: './board.component.html',
  styleUrl: './board.component.scss',
})
export class BoardComponent {
  board: Board = this.boardService.board;

  constructor(
    private readonly boardService: BoardService,
    private readonly boardApiService: BoardApiService
  ) {
    this.boardApiService.moveMade$?.pipe(takeUntilDestroyed()).subscribe((data) => 
      {this.board = (data.value.board); this.boardService.setGameData(data.value)}
    );
    this.boardApiService.gameStarted$?.pipe(takeUntilDestroyed()).subscribe((data) =>
      {this.board = (data.value.board); this.boardService.setGameData(data.value)}
    );
  }

  onTileClick(pos: Position): void {
    this.boardService.makeMoveAndGetGameStatus(pos);
  }
}
