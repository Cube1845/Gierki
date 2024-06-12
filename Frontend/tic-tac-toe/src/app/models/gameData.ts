import { Board } from "./board";
import { Position } from "./position";

export class GameData {
    board: Board = {"boardData":[{"positions": ["", "", ""]}, {"positions": ["", "", ""]}, {"positions": ["", "", ""]}]};
    isGameStarted: boolean = false;
    gameWinnedBy: string | null = null;
    isGameTied: boolean = false;;
    turn: string = "";
    winningTiles: Position[] | null = null;
}