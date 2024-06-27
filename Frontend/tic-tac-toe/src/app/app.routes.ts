import { Routes } from '@angular/router';
import { GameMenuComponent } from './components/game-menu/game-menu.component';
import { GameLobbyComponent } from './components/game-lobby/game-lobby.component';
import { LoginPanelComponent } from './components/login-panel/login-panel.component';
import { RegisterPanelComponent } from './components/register-panel/register-panel.component';

export const routes: Routes = [
    { 'path': 'login', 'component': LoginPanelComponent },
    { 'path': 'register', 'component': RegisterPanelComponent },
    { 'path': 'game', 'component': GameMenuComponent },
    { 'path': 'lobby', 'component': GameLobbyComponent },
    { 'path': '**', 'redirectTo': 'lobby', 'pathMatch': 'full'},
];
