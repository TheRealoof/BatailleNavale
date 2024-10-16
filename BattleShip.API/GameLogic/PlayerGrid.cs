using BattleShip.Models;

namespace BattleShip.API.GameLogic;

public class PlayerGrid
{
    
    public readonly int Width;
    public readonly int Height;
    public readonly int[] ShipLengths;
    
    private readonly HashSet<Ship> _ships = new();
    public IReadOnlyCollection<Ship> Ships => _ships;
    
    public event Action? OnShipAdded; 
    
    public PlayerGrid(GameSettings settings)
    {
        Width = settings.GridWidth;
        Height = settings.GridHeight;
        ShipLengths = settings.ShipLengths;
    }
    
    public void AddShip(Ship ship)
    {
        _ships.Add(ship);
        OnShipAdded?.Invoke();
    }
    
    public bool AllBoatsPlaced()
    {
        HashSet<int> lengths = [..ShipLengths];
        foreach (Ship ship in _ships)
        {
            lengths.Remove(ship.Length);
        }
        return lengths.Count == 0;
    }
    
    public bool IsShipPresent(Coordinates coordinates)
    {
        foreach (Ship ship in _ships)
        {
            if (ship.IsPresent(coordinates))
            {
                return true;
            }
        }
        return false;
    }
    
    public bool IsInBounds(Coordinates coordinates)
    {
        return coordinates.X >= 0 && coordinates.X < Width && coordinates.Y >= 0 && coordinates.Y < Height;
    }

    public void Hit()
    {
        
    }
    
}