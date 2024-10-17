using BattleShip.API.Services;
using BattleShip.Models;

namespace BattleShip.API.GameLogic;

public class QueueManager : IDisposable
{
    private readonly GameService _gameService;

    private readonly HashSet<Player> _quickPlayQueue = new();
    // ReSharper disable once InconsistentNaming
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

    public void JoinQueue(Player player, QueueSettings queueSettings)
    {
        if (_quickPlayQueue.Contains(player) || _againstAIQueue.Contains(player))
        {
            return;
        }

        Enum.TryParse(queueSettings.Type, out QueueType queueType);
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

        Console.WriteLine($"Player {player.Id} joined queue {queueSettings.Type}");
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
        HandleQuickPlayQueue();
    }

    // ReSharper disable once InconsistentNaming
    private void HandleAIQueue()
    {
        while (_againstAIQueue.Count > 0)
        {
            Player player = _againstAIQueue.First();
            LeaveQueue(player);
            GameSettings gameSettings = new GameSettings
            {
                GridWidth = 10,
                GridHeight = 10,
                ShipLengths = [5, 4, 3, 3, 2]
            };
            Game game = new Game(_gameService, gameSettings);
            PlayerController playerController = new PlayerController(game, game.Player1Grid, game.Player2Grid, player);
            _gameService.PlayerControlManager.RegisterPlayerController(playerController);
            AIController aiController = new AIController(game, game.Player2Grid, game.Player1Grid);
            game.Player1Controller = playerController;
            game.Player2Controller = aiController;
            _gameService.GameManager.CreateGame(game);
            _ = _gameService.GameHub.NotifyGameJoined(player.Id, game);
        }
    }

    private void HandleQuickPlayQueue()
    {
        while (_quickPlayQueue.Count >= 2)
        {
            Player player1 = _quickPlayQueue.First();
            LeaveQueue(player1);
            Player player2 = _quickPlayQueue.First();
            LeaveQueue(player2);
            GameSettings gameSettings = new GameSettings
            {
                GridWidth = 10,
                GridHeight = 10,
                ShipLengths = [5, 4, 3, 3, 2]
            };
            Game game = new Game(_gameService, gameSettings);
            PlayerController player1Controller = new PlayerController(game, game.Player1Grid, game.Player2Grid, player1);
            PlayerController player2Controller = new PlayerController(game, game.Player2Grid, game.Player1Grid, player2);
            game.Player1Controller = player1Controller;
            game.Player2Controller = player2Controller;
            _gameService.PlayerControlManager.RegisterPlayerController(player1Controller);
            _gameService.PlayerControlManager.RegisterPlayerController(player2Controller);
            _gameService.GameManager.CreateGame(game);
            _ = _gameService.GameHub.NotifyGameJoined(player1.Id, game);
            _ = _gameService.GameHub.NotifyGameJoined(player2.Id, game);
        }
    }

    private void OnPlayerDisconnected(Player player)
    {
        Console.WriteLine($"Queue Manager: Player {player.Id} disconnected");
        _quickPlayQueue.Remove(player);
        _againstAIQueue.Remove(player);
    }
}