using BattleShip.API.Services;
using BattleShip.Models;

namespace BattleShip.API.GameLogic;

public abstract class BaseQueue<T>
{
    
    private readonly Dictionary<Player, T> _queue = new();
    
    public readonly QueueManager QueueManager;
    public readonly QueueType QueueType;
    
    public GameService GameService => QueueManager.GameService;
    
    public BaseQueue(QueueManager queueManager, QueueType queueType)
    {
        QueueManager = queueManager;
        QueueType = queueType;
    }
    
    public void Add(Player player, T settings)
    {
        if (!_queue.TryAdd(player, settings)) return;
        _ = GameService.GameHub.NotifyJoinQueue(player.Id, QueueType);
        Console.WriteLine($"Player {player.Id} joined queue {QueueType}");
    }
    
    public bool Contains(Player player)
    {
        return _queue.ContainsKey(player);
    }
    
    public void Remove(Player player)
    {
        if (!_queue.Remove(player, out _)) return;
        _ = GameService.GameHub.NotifyLeaveQueue(player.Id);
        Console.WriteLine($"Player {player.Id} left queue {QueueType}");
    }
    
    public int Count => _queue.Count;
    
    public KeyValuePair<Player, T> First()
    {
        return _queue.First();
    }
    
    public void Clear()
    {
        _queue.Clear();
    }

    public abstract void TryMatchPlayers();

}