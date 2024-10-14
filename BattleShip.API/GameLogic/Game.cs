using BattleShip.Models;

namespace BattleShip.API.GameLogic;

public class Game : IDisposable
{
    public readonly Guid Id;

    public readonly GameSettings GameSettings;

    public BaseController? Player1Controller { get; set; }
    public BaseController? Player2Controller { get; set; }

    private BaseController? _currentPlayer;

    private bool _isRunning;
    private readonly Thread _gameThread;

    public Game(GameSettings gameSettings)
    {
        Console.WriteLine("Game created!");
        Id = Guid.NewGuid();
        GameSettings = gameSettings;
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
        Console.WriteLine("Game started!");

        WaitForPlayers();

        // Run game
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
    }

    private void WaitForPlayers()
    {
        Console.WriteLine("Waiting for players to be ready...");

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

        Console.WriteLine("Players are ready!");
    }

    private void HandlePlayer(BaseController playerController)
    {
        // handle player
        _currentPlayer = playerController;
    }
}