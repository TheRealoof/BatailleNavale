using BattleShip.Models;

namespace BattleShip.API;

public class PlayerManager
{
    private Dictionary<Guid, Player> _players = new();

    public Player AddPlayer(string name)
    {
        Guid id = Guid.NewGuid();
        Player player = new Player(id.ToString(), name);
        _players.Add(id, player);
        return player;
    }

    public Player GetPlayer(string id)
    {
        return _players[Guid.Parse(id)];
    }

    public void RemovePlayer(string id)
    {
        _players.Remove(Guid.Parse(id));
    }
}