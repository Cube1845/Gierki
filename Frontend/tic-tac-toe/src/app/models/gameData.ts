import { Board } from "./board";
import { PositionCollection } from "./positionCollection";
import { UserRole } from "./userRole";

export class GameData {
    id: number | null = null;
    board: Board = {
        "boardData": [
            {"positions": ["", "", ""]}, 
            {"positions": ["", "", ""]}, 
            {"positions": ["", "", ""]}
        ]
    };
    
    isGameStarted: boolean = false;
    gameWinnedBy: string | null = null;
    isGameTied: boolean = false;
    turn: string = "";
    winningTiles: PositionCollection | null = null;
    players: UserRole[] = [];
}