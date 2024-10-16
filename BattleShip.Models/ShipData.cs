namespace BattleShip.Models;

public class ShipData
{
    public Coordinates Coordinates { get; init; }
    public int Length { get; init; }
    public ShipDirection Direction { get; init; }
    public bool Alive { get; set; } = true;
}