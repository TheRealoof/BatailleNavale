namespace BattleShip.Models;

public class GameSettings
{
    public int GridWidth { get; init; } = 1;
    public int GridHeight { get; init; } = 1;
    public int[] ShipLengths { get; init; } = [];
}