import { Injectable } from '@angular/core';
import { LoginUser } from '../models/loginUser';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Result } from '../models/result';
import { AppUser } from '../models/appUser';

@Injectable({
  providedIn: 'root'
})
export class AuthApiService {
  constructor(private readonly http: HttpClient) {}

  private readonly apiUrl: string = "https://localhost:7047/Auth";

  loginWithUsernameAndData(user: LoginUser): Observable<Result> {
    return this.http.post<Result>(this.apiUrl + '/Login', user);
  }

  register(user: AppUser): Observable<Result> {
    return this.http.post<Result>(this.apiUrl + '/RegisterUser', user);
  }
}
