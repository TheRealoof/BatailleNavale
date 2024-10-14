using BattleShip.Models;

namespace BattleShip.API.GameLogic;

public class GameManager
{
    private readonly Dictionary<Guid, Game> _lobbies = new();

    public void CreateGame(Game game)
    {
        _lobbies.Add(game.Id, game);
        Console.WriteLine($"Game created: {game.Id}");
    }

    public Game GetGame(string id)
    {
        return _lobbies[Guid.Parse(id)];
    }

    public void RemoveGame(string id)
    {
        _lobbies.Remove(Guid.Parse(id));
    }

    public void PerformAttack(string gameId, string playerId, uint x, uint y)
    {
        Game game = GetGame(gameId);
    }
}