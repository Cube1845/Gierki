import { Component } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Move } from '../../models/move';
import { BoardApiService } from '../../services/board-api.service';
import { BoardService } from '../../services/board.service';
import { TileComponent } from '../tile/tile.component';

@Component({
  selector: 'app-board',
  standalone: true,
  imports: [TileComponent],
  templateUrl: './board.component.html',
  styleUrl: './board.component.scss',
})
export class BoardComponent {
  constructor(
    private readonly boardService: BoardService,
    private readonly boardApiService: BoardApiService
  ) {
    this.boardApiService.moveMade$.pipe(takeUntilDestroyed()).subscribe((x) => {
      console.log(x);
    });
  }

  board: string[][] = this.boardService.board;
  responseText: string = '';

  getBoard(): string[][] {
    return this.boardService.getBoard();
  }

  onTileClick(move: Move): void {
    this.boardApiService.makeMoveAndGetGameStatus(move);
  }
}
