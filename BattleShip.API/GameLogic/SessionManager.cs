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
        if (!_sessions.TryGetValue(connectionId, out Player? player))
        {
            return;
        }
        Console.WriteLine($"Player disconnected: {player.Id}");
        OnPlayerDisconnected?.Invoke(player);
        _connectionIds.Remove(player.Id);
        _sessions.Remove(connectionId);
    }
    
    public string? GetConnectionId(string playerId)
    {
        return _connectionIds.GetValueOrDefault(playerId);
    }
    
    public Player GetPlayer(string connectionId)
    {
        return _sessions[connectionId];
    }
    
    public bool IsPlayerConnected(string playerId)
    {
        return _connectionIds.ContainsKey(playerId);
    }

    public List<Player> GetConnectedPlayers()
    {
        return _sessions.Values.ToList();
    }
}