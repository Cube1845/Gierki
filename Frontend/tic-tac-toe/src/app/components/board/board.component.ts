import { NgFor } from '@angular/common';
import { Component } from '@angular/core';
import { take } from 'rxjs';
import { BoardApiService } from '../../services/board-api.service';
import { BoardService } from '../../services/board.service';

@Component({
  selector: 'app-board',
  standalone: true,
  imports: [NgFor],
  templateUrl: './board.component.html',
  styleUrl: './board.component.scss',
})
export class BoardComponent {
  constructor(
    private readonly boardService: BoardService,
    private readonly boardApiService: BoardApiService
  ) {}

  board: string[][] = this.boardService.board;
  responseText: string = '';

  getBoard(): string[][] {
    return this.boardService.getBoard();
  }

  tileClick(event: any): void {
    var symbol = event.srcElement.innerHTML;
    if (symbol != 'X' && symbol != 'O') {
      var y = event.srcElement.id[5];
      var x = event.srcElement.id[7];
      var turn = this.boardService.getTurn();

      this.boardApiService.makeMoveAndGetGameStatus(
        {
        symbol: "O", 
        position: {
          x: x,
          y: y
        }
      });
    }
  }
}
