using BattleShip.Models;

namespace BattleShip.API.GameLogic;

public class Ship
{
    private bool Alive { get; set; } = true;

    public readonly Coordinates Coordinates;

    public readonly int Length;

    public readonly ShipDirection Direction;

    public readonly List<Coordinates> CoordinatesList;

    public Ship(int positionX, int positionY, int length, ShipDirection direction)
    {
        Coordinates = new Coordinates(positionX, positionY);
        Length = length;
        Direction = direction;
        CoordinatesList = [];
        ComputeCoordinates();
    }
    
    public void ComputeCoordinates()
    {
        bool vertical = Direction == ShipDirection.Down || Direction == ShipDirection.Up;
        bool negative = Direction == ShipDirection.Up || Direction == ShipDirection.Left;

        if (vertical)
        {
            for (int i = 0; i < Length; i++)
            {
                int y = negative ? -i : i;
                CoordinatesList.Add(Coordinates + new Coordinates(0, y));
            }
        }
        else
        {
            for (int i = 0; i < Length; i++)
            {
                int x = negative ? -i : i;
                CoordinatesList.Add(Coordinates + new Coordinates(x, 0));
            }
        }
    }
    
    public bool IsPresent(Coordinates coordinates)
    {
        return CoordinatesList.Any(c => c == coordinates);
    }
    
}