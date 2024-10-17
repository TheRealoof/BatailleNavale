namespace BattleShip.Models;

public class GameData
{
    public string Id { get; init; } = "";
    public GameSettings Settings { get; init; } = new();
}