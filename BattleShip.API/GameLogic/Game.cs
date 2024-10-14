using BattleShip.Models;

namespace BattleShip.API.GameLogic;

public class Game : IDisposable
{
    public readonly Guid Id;

    public readonly GameSettings GameSettings;
    
    public GameState State { get; private set; }

    public BaseController? Player1Controller { get; set; }
    public BaseController? Player2Controller { get; set; }

    private BaseController? _currentPlayer;

    private bool _isRunning;
    private readonly Thread _gameThread;

    public Game(GameSettings gameSettings)
    {
        Id = Guid.NewGuid();
        GameSettings = gameSettings;
        State = GameState.WaitingForPlayers;
        _isRunning = true;
        _gameThread = new Thread(RunGame);
        _gameThread.Start();
    }

    public void Dispose()
    {
        if (_currentPlayer is not null)
        {
            // TODO cancel current player's turn
        }

        _isRunning = false;
        _gameThread.Join();
    }

    private void RunGame()
    {
        State = GameState.WaitingForPlayers;
        Console.WriteLine("Waiting for players to be ready...");
        WaitForPlayers();
        
        State = GameState.PlacingShips;
        Console.WriteLine("Placing ships...");
        PlaceShips();
        
        State = GameState.Playing;
        Console.WriteLine("Game started!");
        while (_isRunning)
        {
            // handle player 1
            while (Player1Controller is null)
            {
                Thread.Sleep(100);
            }

            HandlePlayer(Player1Controller);
            // handle player 2
            while (Player2Controller is null)
            {
                Thread.Sleep(100);
            }

            HandlePlayer(Player2Controller);
        }
        
        State = GameState.GameOver;
    }

    private void WaitForPlayers()
    {
        while (_isRunning)
        {
            bool player1Ready = !(Player1Controller is null || !Player1Controller.IsReady);
            bool player2Ready = !(Player2Controller is null || !Player2Controller.IsReady);

            if (player1Ready && player2Ready)
            {
                break;
            }

            Thread.Sleep(100);
        }
    }
    
    private void PlaceShips()
    {
        
    }

    private void HandlePlayer(BaseController playerController)
    {
        // handle player
        _currentPlayer = playerController;
    }
}