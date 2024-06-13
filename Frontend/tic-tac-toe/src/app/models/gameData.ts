import { Board } from "./board";
import { PositionCollection } from "./positionCollection";

export class GameData {
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
}