using BattleShip.Models;

namespace BattleShip.API.GameLogic;

public class GameManager : IDisposable
{
    private readonly Dictionary<Guid, Game> _lobbies = new();

    public void Dispose()
    {
        foreach (Game game in _lobbies.Values)
        {
            game.Dispose();
        }
    }

    public void CreateGame(Game game)
    {
        _lobbies.Add(game.Id, game);
        game.OnStop += () => RemoveGame(game.Id.ToString());
        game.Start();
        Console.WriteLine($"Game created: {game.Id}");
    }

    public Game? GetGame(string id)
    {
        _lobbies.TryGetValue(Guid.Parse(id), out Game? game);
        return game;
    }

    public void RemoveGame(string id)
    {
        _lobbies.Remove(Guid.Parse(id));
        Console.WriteLine($"Removed game: {id}");
    }
    
}