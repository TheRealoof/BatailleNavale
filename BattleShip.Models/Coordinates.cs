namespace BattleShip.Models;

public class Coordinates
{
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;

    public Coordinates()
    {
    }

    public Coordinates(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Coordinates(Coordinates coordinates)
    {
        X = coordinates.X;
        Y = coordinates.Y;
    }

    public static bool operator ==(Coordinates c1, Coordinates c2)
    {
        return c1.X == c2.X && c1.Y == c2.Y;
    }

    public static bool operator !=(Coordinates c1, Coordinates c2)
    {
        return !(c1 == c2);
    }

    public static Coordinates operator +(Coordinates c1, Coordinates c2)
    {
        return new Coordinates(c1.X + c2.X, c1.Y + c2.Y);
    }

    public static Coordinates operator -(Coordinates c1, Coordinates c2)
    {
        return new Coordinates(c1.X - c2.X, c1.Y - c2.Y);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Coordinates c)
        {
            return this == c;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() ^ Y.GetHashCode();
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}