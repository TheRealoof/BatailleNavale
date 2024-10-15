namespace BattleShip.Models;

public class Boat
{
    public int PositionX { get; init; }
    public int PositionY { get; init; }
    public int Length { get; init; }
    public BoatDirection Direction { get; init; }
    public bool Alive { get; set; } = true;
}