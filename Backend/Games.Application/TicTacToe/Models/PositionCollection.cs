namespace Games.Application.TicTacToe.Models;

public class PositionCollection
{
    public Position T1 { get; private set; }
    public Position T2 { get; private set; }
    public Position T3 { get; private set; }

    public PositionCollection(int[] t1, int[] t2, int[] t3)
    {
        T1 = t1;
        T2 = t2;
        T3 = t3;
    }
}