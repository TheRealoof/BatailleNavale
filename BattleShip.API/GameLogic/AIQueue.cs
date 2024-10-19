using BattleShip.Models;

namespace BattleShip.API.GameLogic;

// ReSharper disable once InconsistentNaming
public class AIQueue : BaseQueue<AIQueue.AIQueueSettings>
{
    // ReSharper disable once InconsistentNaming
    public class AIQueueSettings
    {
        public int Difficulty { get; init; }
    }
    
    // ReSharper disable once ConvertToPrimaryConstructor
    public AIQueue(QueueManager queueManager) : base(queueManager, QueueType.AgainstAI)
    {
    }

    public override void TryMatchPlayers()
    {
        while (Count > 0)
        {
            (Player player, AIQueueSettings settings) = First();
            Remove(player);
            StartGame(player, settings);
        }
    }
    
    private void StartGame(Player player, AIQueueSettings settings)
    {
        GameSettings gameSettings;
        if (settings.Difficulty == 0)
        {
            gameSettings = new GameSettings
            {
                GridWidth = 10,
                GridHeight = 10,
                ShipLengths = [5, 4, 3, 3, 2]
            };
        }
        else
        {
            gameSettings = new GameSettings
            {
                GridWidth = 12,
                GridHeight = 12,
                ShipLengths = [4, 3, 3, 3, 2, 2, 2]
            };
        }
        Game game = new Game(GameService, gameSettings);
        PlayerController playerController = new PlayerController(game, game.Player1Grid, game.Player2Grid, player);
        GameService.PlayerControlManager.RegisterPlayerController(playerController);
        AIController aiController = new AIController(
            game, game.Player2Grid, game.Player1Grid,
            (settings.Difficulty == 0) ? AIController.AIDifficulty.Easy : AIController.AIDifficulty.Hard
        );
        game.Player1Controller = playerController;
        game.Player2Controller = aiController;
        GameService.GameManager.CreateGame(game);
        _ = GameService.GameHub.NotifyGameJoined(player.Id, game);
    }
}