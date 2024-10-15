using BattleShip.Models;

namespace BattleShip.API.GameLogic;

public class PlayerGrid
{
    
    public readonly int Width;
    public readonly int Height;
    public readonly int[] ShipLengths;
    
    private readonly HashSet<Ship> _ships = new();
    public IReadOnlyCollection<Ship> Ships => _ships;
    
    public PlayerGrid(GameSettings settings)
    {
        Width = settings.GridWidth;
        Height = settings.GridHeight;
        ShipLengths = settings.ShipLengths;
    }
    
    public void AddShip(Ship ship)
    {
        _ships.Add(ship);
    }

    public bool IsPresent(int x, int y)
    {
        return true;
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

    public void Hit()
    {
        
    }
    
}