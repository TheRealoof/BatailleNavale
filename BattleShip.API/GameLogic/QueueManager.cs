using BattleShip.API.Services;
using BattleShip.Models;

namespace BattleShip.API;

public class QueueManager : IDisposable
{
    private readonly GameService _gameService;

    private readonly HashSet<Player> _quickPlayQueue = new();
    private readonly HashSet<Player> _againstAIQueue = new();

    private bool _isRunning;
    private readonly Thread _queueThread;

    public QueueManager(GameService gameService)
    {
        _gameService = gameService;
        _gameService.SessionManager.OnPlayerDisconnected += OnPlayerDisconnected;
        _isRunning = true;
        _queueThread = new Thread(RunQueueThread);
        _queueThread.Start();
    }

    public void Dispose()
    {
        _isRunning = false;
        _queueThread.Join();
    }

    public void JoinQueue(Player player, QueueType queueType)
    {
        if (_quickPlayQueue.Contains(player) || _againstAIQueue.Contains(player))
        {
            return;
        }

        switch (queueType)
        {
            case QueueType.QuickPlay:
            {
                _quickPlayQueue.Add(player);
                break;
            }
            case QueueType.AgainstAI:
            {
                _againstAIQueue.Add(player);
                break;
            }
        }

        Console.WriteLine($"Player {player.Id} joined queue {queueType}");
        // send signalR message to client
        _ = _gameService.GameHub.NotifyJoinQueue(player.Id, queueType);
    }

    // leave queue
    public void LeaveQueue(Player player)
    {
        if (
            !_quickPlayQueue.Remove(player) &&
            !_againstAIQueue.Remove(player)
        ) return;
        
        Console.WriteLine($"Player {player.Id} left queue");
        // send signalR message to client
        _ = _gameService.GameHub.NotifyLeaveQueue(player.Id);
    }

    private void RunQueueThread()
    {
        while (_isRunning)
        {
            HandleQueues();
            Thread.Sleep(1000);
        }
    }

    private void HandleQueues()
    {
        Console.WriteLine($"QuickPlayQueue: {_quickPlayQueue.Count}");
        Console.WriteLine($"AgainstAIQueue: {_againstAIQueue.Count}");
        HandleAIQueue();
    }

    // ReSharper disable once InconsistentNaming
    private void HandleAIQueue()
    {
        while (_againstAIQueue.Count > 0)
        {
            Player player = _againstAIQueue.First();
            LeaveQueue(player);
            PlayerController playerController = new PlayerController();
            AIController aiController = new AIController();
            Game game = new Game
            {
                Player1Controller = playerController,
                Player2Controller = aiController
            };
            _gameService.GameManager.CreateGame(game);
            _ = _gameService.GameHub.NotifyGameJoined(player.Id, game);
        }
    }

    private void OnPlayerDisconnected(Player player)
    {
        Console.WriteLine($"Queue Manager: Player {player.Id} disconnected");
        _quickPlayQueue.Remove(player);
        _againstAIQueue.Remove(player);
    }
}