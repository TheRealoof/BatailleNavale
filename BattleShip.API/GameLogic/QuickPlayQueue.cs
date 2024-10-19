using BattleShip.Models;

namespace BattleShip.API.GameLogic;

public class QuickPlayQueue : BaseQueue<object?>
{
    // ReSharper disable once ConvertToPrimaryConstructor
    public QuickPlayQueue(QueueManager queueManager) : base(queueManager, QueueType.QuickPlay)
    {
    }

    public override void TryMatchPlayers()
    {
        while (Count >= 2)
        {
            (Player player1, object? _) = First();
            Remove(player1);
            (Player player2, object? _) = First();
            Remove(player2);
            StartGame(player1, player2);
        }
    }

    private void StartGame(Player player1, Player player2)
    {
        GameSettings gameSettings = new GameSettings
        {
            GridWidth = 10,
            GridHeight = 10,
            ShipLengths = [5, 4, 3, 3, 2]
        };
        Game game = new Game(GameService, gameSettings);
        PlayerController player1Controller =
            new PlayerController(game, game.Player1Grid, game.Player2Grid, player1);
        PlayerController player2Controller =
            new PlayerController(game, game.Player2Grid, game.Player1Grid, player2);
        game.Player1Controller = player1Controller;
        game.Player2Controller = player2Controller;
        GameService.PlayerControlManager.RegisterPlayerController(player1Controller);
        GameService.PlayerControlManager.RegisterPlayerController(player2Controller);
        GameService.GameManager.CreateGame(game);
        _ = GameService.GameHub.NotifyGameJoined(player1.Id, game);
        _ = GameService.GameHub.NotifyGameJoined(player2.Id, game);
    }
    
}