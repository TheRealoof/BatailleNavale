namespace BattleShip.Models;

public class ShipData
{
    public Coordinates Coordinates { get; init; } = new();
    public int Length { get; init; } = 1;
    public ShipDirection Direction { get; init; }
    public bool Alive { get; set; } = true;
}