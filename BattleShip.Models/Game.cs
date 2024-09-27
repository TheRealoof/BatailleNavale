namespace BattleShip.Models;

public class Game
{
    public static readonly int MaxPlayers = 2;
    public readonly string Id;
    public readonly Grid[] Grids = new Grid[MaxPlayers];

    public Game(string id)
    {
        Id = id;
        Grids[0] = new Grid(10, 10);
        Grids[1] = new Grid(10, 10);
    }
}