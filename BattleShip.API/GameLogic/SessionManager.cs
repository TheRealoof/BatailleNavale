using BattleShip.Models;

namespace BattleShip.API.GameLogic;

public class SessionManager
{
    private readonly Dictionary<string, Player> _sessions = new();
    private readonly Dictionary<string, string> _connectionIds = new();
    
    public event Action<Player>? OnPlayerDisconnected;

    public void PlayerConnected(string connectionId, Player player)
    {
        _sessions.Add(connectionId, player);
        _connectionIds.Add(player.Id, connectionId);
        Console.WriteLine($"Player connected: {player.Id}");
    }

    public void PlayerDisconnected(string connectionId)
    {
        Console.WriteLine($"Player disconnected: {_sessions[connectionId].Id}");
        OnPlayerDisconnected?.Invoke(_sessions[connectionId]);
        _connectionIds.Remove(_sessions[connectionId].Id);
        _sessions.Remove(connectionId);
    }
    
    public string? GetConnectionId(string playerId)
    {
        return _connectionIds.GetValueOrDefault(playerId);
    }

    public List<Player> GetConnectedPlayers()
    {
        return _sessions.Values.ToList();
    }
}