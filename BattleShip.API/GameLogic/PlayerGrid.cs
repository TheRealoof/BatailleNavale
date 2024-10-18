using BattleShip.Models;

namespace BattleShip.API.GameLogic;

public class PlayerGrid
{
    
    public readonly int Width;
    public readonly int Height;
    public readonly int[] ShipLengths;
    
    private readonly HashSet<Ship> _ships = new();
    public IReadOnlyCollection<Ship> Ships => _ships;
    private readonly HashSet<Coordinates> _attackedCoordinates = new();
    public IReadOnlyCollection<Coordinates> AttackedCoordinates => _attackedCoordinates;
    public IReadOnlyCollection<Ship> SunkenShips => Ships.Where(IsShipSunk).ToList();
    
    public event Action? OnUpdate; 
    
    public PlayerGrid(GameSettings settings)
    {
        Width = settings.GridWidth;
        Height = settings.GridHeight;
        ShipLengths = settings.ShipLengths;
    }
    
    public void AddShip(Ship ship)
    {
        if (!GetRemainingShipLengths().Contains(ship.Length))
        {
            return;
        }
        _ships.Add(ship);
        OnUpdate?.Invoke();
    }
    
    public bool AllBoatsPlaced()
    {
        return GetRemainingShipLengths().Count == 0;
    }
    
    private List<int> GetRemainingShipLengths()
    {
        List<int> lengths = [..ShipLengths];
        foreach (Ship ship in _ships)
        {
            lengths.Remove(ship.Length);
        }
        return lengths;
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
    
    public bool CanAttack(Coordinates coordinates)
    {
        return !AttackedCoordinates.Contains(coordinates);
    }

    public void Attack(Coordinates coordinates)
    {
        if (!CanAttack(coordinates))
        {
            return;
        }
        _attackedCoordinates.Add(coordinates);
        OnUpdate?.Invoke();
    }
    
    public bool IsShipSunk(Ship ship)
    {
        foreach (Coordinates coordinates in ship.CoordinatesList)
        {
            if (!AttackedCoordinates.Contains(coordinates))
            {
                return false;
            }
        }
        return true;
    }
    
}