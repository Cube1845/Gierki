export class Move {
    symbol: string;
    position: number[];

    constructor(symbol: string, position: number[]) {
        this.symbol = symbol;
        this.position = position;
    }
}