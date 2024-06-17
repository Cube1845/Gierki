import { Component, OnInit } from '@angular/core';
import { BoardApiService } from '../../services/board-api.service';
import { BoardService } from '../../services/board.service';
import { NgIf } from '@angular/common';
import { BoardComponent } from '../board/board.component';
import { ActivatedRoute, Router, } from '@angular/router';
import { LobbyApiService } from '../../services/lobby-api.service';

@Component({
  selector: 'app-game-menu',
  standalone: true,
  imports: [ NgIf, BoardComponent ],
  templateUrl: './game-menu.component.html',
  styleUrl: './game-menu.component.scss'
})

export class GameMenuComponent implements OnInit {
  private connectionId!: string;

  constructor(private readonly boardApiService: BoardApiService, private readonly boardService: BoardService, private readonly route: ActivatedRoute, private readonly router: Router,private readonly lobbyApiService: LobbyApiService) {
    this.connectionId = this.route.snapshot.params['name'];
  }

  async ngOnInit(): Promise<void> {
    await this.boardApiService.startConnection();
    this.boardApiService.loadGameDataAndUpdateConnectionId(this.connectionId);
  }

  getWinner(): string | null{
    if (this.boardService.getWinner() != null) {
      return this.boardService.getWinner();
    } else {
      return "";
    }
  }

  goBackToLobby(): void {
    this.boardApiService.stopConnection();
    this.router.navigateByUrl('/lobby');
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

  getThisUsersTurn(): string {
    return this.boardService.getThisUsersTurn();
  }
}
