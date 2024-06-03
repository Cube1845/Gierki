import { Component } from '@angular/core';
import { NgFor } from '@angular/common';
import { BoardService } from '../../services/board.service';

@Component({
  selector: 'app-board',
  standalone: true,
  imports: [ NgFor ],
  templateUrl: './board.component.html',
  styleUrl: './board.component.scss'
})

export class BoardComponent {
  constructor(private readonly boardService: BoardService) {}

  board: string[][] = this.boardService.board;
  
  tileClick(event: any): void {
    if (this.boardService.getGameStatus() && event.srcElement.firstChild.data == "") {
      var id = event.srcElement.id;
      var x: number = Number(id[7]);
      var y: number = Number(id[5]);
      this.boardService.makeMove([y, x]);
    }
  }
}
