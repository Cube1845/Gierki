import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { Move } from '../models/move';
import { Result } from '../models/result';
import { BoardService } from './board.service';

@Injectable({
  providedIn: 'root',
})
export class BoardApiService {
  apiUrl = 'https://localhost:7047/Move';

  constructor(
    private readonly http: HttpClient,
    private readonly boardService: BoardService
  ) {}

  startGame(): Observable<string> {
    return this.http
      .get<Result>(`${this.apiUrl}/StartGame`)
      .pipe(map((response) => response.message!));
  }

  sendMove(move: Move): Observable<Result<string[][]>> {
    return this.http.post<Result<string[][]>>(
      `${this.apiUrl}/MakeMoveAndCheckGameStatus`,
      move
    );
  }
}
