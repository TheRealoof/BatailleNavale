using BattleShip.Models;

namespace BattleShip.API.GameLogic;

public class Ship
{
    private bool Alive { get; set; } = true;

    public readonly int PositionX;
    public readonly int PositionY;

    public readonly int Length;

    public readonly BoatDirection Direction;

    public Ship(int positionX, int positionY, int length, BoatDirection direction)
    {
        PositionX = positionX;
        PositionY = positionY;
        Length = length;
        Direction = direction;
    }

    public bool CheckHit(int x, int y)
    {
        // Check if the boat is still alive
        if (!Alive)
        {
            return false;
        }

        // Check if the hit is within the boat's bounds
        bool vertical = Direction == BoatDirection.Down || Direction == BoatDirection.Up;
        bool negative = Direction == BoatDirection.Up || Direction == BoatDirection.Left;
        int direction = (negative ? -1 : 1) * Length;

        if (vertical)
        {
            if (x != PositionX)
            {
                return false;
            }

            for (int i = PositionY; i != PositionY + direction; i += negative ? -1 : 1)
            {
                if (i == y)
                {
                    return true;
                }
            }
        }
        else
        {
            if (y != PositionY)
            {
                return false;
            }

            for (int i = PositionX; i != PositionX + direction; i += negative ? -1 : 1)
            {
                if (i == x)
                {
                    return true;
                }
            }
        }

        return false;
    }
}