namespace Games.Application.TicTacToe.Models;

public class PositionCollection
{
    public List<Position> Positions { get; private set; }

    public PositionCollection(List<Position> positions)
    {
        Positions = positions;
    }
}