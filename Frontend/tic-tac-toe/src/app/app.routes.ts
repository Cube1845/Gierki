import { Routes } from '@angular/router';
import { GameMenuComponent } from './components/game-menu/game-menu.component';
import { GameLobbyComponent } from './components/game-lobby/game-lobby.component';

export const routes: Routes = [ 
    { 'path': 'game/:name', 'component': GameMenuComponent },
    { 'path': 'lobby', 'component': GameLobbyComponent },
    { 'path': '**', 'redirectTo': 'lobby', 'pathMatch': 'full'},
];
