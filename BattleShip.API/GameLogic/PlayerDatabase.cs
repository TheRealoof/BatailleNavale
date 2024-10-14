using BattleShip.Models;

namespace BattleShip.API.GameLogic;

public class PlayerDatabase
{
    private readonly Dictionary<string, Player> _players = new();

    public Player GetOrCreatePlayer(string id)
    {
        if (_players.TryGetValue(id, out Player? createPlayer))
        {
            return createPlayer;
        }

        Player player = new Player
        {
            Id = id
        };
        _players.Add(id, player);
        return player;
    }
}