using BattleShip.API.Services;
using BattleShip.Models;

namespace BattleShip.API.GameLogic;

public class QueueManager : IDisposable
{
    public readonly GameService GameService;

    private readonly QuickPlayQueue _quickPlayQueue;

    // ReSharper disable once InconsistentNaming
    private readonly AIQueue _againstAiQueue;

    private bool _isRunning;
    private readonly Thread _queueThread;

    public QueueManager(GameService gameService)
    {
        GameService = gameService;
        _quickPlayQueue = new QuickPlayQueue(this);
        _againstAiQueue = new AIQueue(this);
        GameService.SessionManager.OnPlayerDisconnected += OnPlayerDisconnected;
        _isRunning = true;
        _queueThread = new Thread(RunQueueThread);
        _queueThread.Start();
    }

    public void Dispose()
    {
        _isRunning = false;
        _queueThread.Join();
    }

    public void JoinQueue(Player player, QueueSettings queueSettings)
    {
        if (_quickPlayQueue.Contains(player) || _againstAiQueue.Contains(player))
        {
            return;
        }

        Enum.TryParse(queueSettings.Type, out QueueType queueType);
        switch (queueType)
        {
            case QueueType.QuickPlay:
            {
                _quickPlayQueue.Add(player, null);
                break;
            }
            case QueueType.AgainstAI:
            {
                int difficulty = queueSettings.AIDifficulty ?? 1;
                _againstAiQueue.Add(player, new AIQueue.AIQueueSettings
                {
                    Difficulty = difficulty
                });
                break;
            }
        }
    }

    // leave queue
    public void LeaveQueue(Player player)
    {
        _quickPlayQueue.Remove(player);
        _againstAiQueue.Remove(player);
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
        _againstAiQueue.TryMatchPlayers();
        _quickPlayQueue.TryMatchPlayers();
    }

    private void OnPlayerDisconnected(Player player)
    {
        LeaveQueue(player);
    }
}