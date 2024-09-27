using BattleShip.Models;

namespace BattleShip.API;

public class GameManager
{
    private readonly Dictionary<Guid, Game> _lobbies = new();

    public Game CreateGame()
    {
        Guid id = Guid.NewGuid();
        Game game = new Game(id.ToString());
        _lobbies.Add(id, game);
        return game;
    }

    public Game GetGame(string id)
    {
        return _lobbies[Guid.Parse(id)];
    }

    public void RemoveGame(string id)
    {
        _lobbies.Remove(Guid.Parse(id));
    }
    
    // bind player to grid (indicate who the grid belongs to)
    public void BindPlayerToGrid(string playerId, string gameId, uint gridIndex)
    {
        
    }
    

    public void PerformAttack(string gameId, string playerId, uint x, uint y)
    {
        Game game = GetGame(gameId);
    }
}