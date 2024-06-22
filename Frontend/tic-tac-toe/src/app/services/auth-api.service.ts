import { Injectable } from '@angular/core';
import { LoginDto } from '../models/loginDto';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { Result } from '../models/result';
import { AppUser } from '../models/appUser';

@Injectable({
  providedIn: 'root'
})
export class AuthApiService {
  constructor(private readonly http: HttpClient) {}

  private readonly apiUrl: string = "https://localhost:7047/Auth";

  loginWithUsernameAndData(user: LoginDto): Observable<Result> {
    return this.http.post<Result>(this.apiUrl + '/Login', user);
  }

  register(user: AppUser): Observable<Result> {
    return this.http.post<Result>(this.apiUrl + '/RegisterUser', user);
  }

  isUserAuthenticated(): Observable<boolean> {
    var token = localStorage.getItem("token")!;

    var header = new HttpHeaders().set(
      "Authorization",
       localStorage.getItem(token)!
    );

    return this.http.get<boolean>(this.apiUrl + '/IsUserAuthorized', { headers: header });
  }
}
