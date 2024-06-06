namespace Games.Application.TicTacToe.Models;

public record Position(int X, int Y)
{
    public static implicit operator Position(int[] coords)
    {
        if (coords.Length != 2 && coords.Any(c => c < 0 || c > 2)) 
        {
            throw new ArgumentException("Coordinates must have length = 2 and have values between o and 2");
        }

        return new(coords[1], coords[0]);
    }
}