import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map, tap } from 'rxjs';
import { BoardService } from './board.service';
import { Move } from '../models/move';
import { Response } from '../models/response';

@Injectable({
  providedIn: 'root'
})
export class BoardApiService {

  apiUrl = "https://localhost:7047/Move"

  constructor(private readonly http: HttpClient, private readonly boardService: BoardService) {}

  startGame(): Observable<string> {
    return this.http.get<Response>(`${this.apiUrl}/StartGame`).pipe(
      map((response) => response.responseText)
    );
  }

  sendMove(move: Move): Observable<Response> {
    return this.http.post<Response>(`${this.apiUrl}/MakeMoveAndCheckGameStatus`, move);
  }
}
