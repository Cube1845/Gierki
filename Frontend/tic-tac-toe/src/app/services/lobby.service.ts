import { Injectable } from '@angular/core';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class LobbyService {
  waitingUsers: User[] = [];
  isUserWaiting: boolean = false;
  thisUser: User = {'connectionId': '', 'username': ''};

  getWaitingUsernames(): string[] {
    var users: string[] = [];
    this.waitingUsers.forEach(user => {
      if (user.username == this.thisUser.username) {
        users.push(user.username + " (you)");
      } else {
        users.push(user.username);
      }
    });

    return users;
  }

  setThisUser(user: User): void {
    this.thisUser = user;
  }

  isThisUserWaiting(): boolean {
    return this.isUserWaiting;
  }

  setThisUserWaiting(value: boolean): void {
    this.isUserWaiting = value;
  }

  setWaitingUsers(users: User[]): void {
    this.waitingUsers = users;
  }

  getWaitingUsersCount(): number {
    return this.waitingUsers.length;
  }
}
