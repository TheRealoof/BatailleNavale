namespace BattleShip.Models;

public class GridData
{
    public bool IsSelf { get; init; }
    public List<ShipData> ShipData { get; init; } = [];
    public List<Coordinates> HitCoordinates { get; init; } = [];
    public List<Coordinates> MissCoordinates { get; init; } = [];
}