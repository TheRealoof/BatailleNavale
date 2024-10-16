using BattleShip.API.Services;
using BattleShip.Models;

namespace BattleShip.API.GameLogic;

public class Game : IDisposable
{
    public readonly GameService GameService;

    public readonly Guid Id;

    public readonly GameSettings GameSettings;

    public GameState State { get; private set; }

    public BaseController Player1Controller { get; set; } = null!;
    public BaseController Player2Controller { get; set; } = null!;

    private BaseController? _currentPlayer;

    public readonly PlayerGrid Player1Grid;
    public readonly PlayerGrid Player2Grid;

    private bool _isRunning;
    private readonly Thread _gameThread;

    public Game(GameService gameService, GameSettings gameSettings)
    {
        GameService = gameService;

        Id = Guid.NewGuid();
        GameSettings = gameSettings;
        Player1Grid = new PlayerGrid(gameSettings);
        Player2Grid = new PlayerGrid(gameSettings);
        _isRunning = false;
        _gameThread = new Thread(RunGame);
    }

    public void Start()
    {
        State = GameState.WaitingForPlayers;
        _isRunning = true;
        _gameThread.Start();
    }
    
    public void Stop()
    {
        if (_currentPlayer is not null)
        {
            // TODO cancel current player's turn
        }
        _isRunning = false;
        _gameThread.Join();
    }

    public void Dispose()
    {
        Stop();
    }

    private void RunGame()
    {
        State = GameState.WaitingForPlayers;
        NotifyStateChange();
        Console.WriteLine("Waiting for players to be ready...");
        WaitForPlayers();
        
        if (!_isRunning) return;

        State = GameState.PlacingShips;
        NotifyStateChange();
        Console.WriteLine("Placing ships...");
        PlaceShips();
        
        if (!_isRunning) return;

        State = GameState.Playing;
        NotifyStateChange();
        Console.WriteLine("Game started!");
        GameLogic();
        
        if (!_isRunning) return;

        State = GameState.GameOver;
        NotifyStateChange();
    }

    private void WaitForPlayers()
    {
        while (_isRunning)
        {
            bool player1Ready = Player1Controller.IsReady;
            bool player2Ready = Player2Controller.IsReady;

            if (player1Ready && player2Ready)
            {
                break;
            }

            Thread.Sleep(100);
        }
    }

    private void PlaceShips()
    {
        Player1Controller.CanPlaceShips = true;
        Player2Controller.CanPlaceShips = true;
        while (_isRunning)
        {
            bool player1Ready = Player1Grid.AllBoatsPlaced();
            bool player2Ready = Player2Grid.AllBoatsPlaced();

            if (player1Ready && player2Ready)
            {
                break;
            }

            Thread.Sleep(100);
        }
        Player1Controller.CanPlaceShips = false;
        Player2Controller.CanPlaceShips = false;
        
        // log player 1 ships
        Console.WriteLine("Player 1 ships:");
        foreach (Ship ship in Player1Grid.Ships)
        {
            Console.WriteLine($"Ship placed at {ship.Coordinates}, direction: {ship.Direction}, length: {ship.Length}");
        }
        Console.WriteLine("Player 2 ships:");
        foreach (Ship ship in Player2Grid.Ships)
        {
            Console.WriteLine($"Ship placed at {ship.Coordinates}, direction: {ship.Direction}, length: {ship.Length}");
        }
    }

    private void GameLogic()
    {
        while (_isRunning)
        {
            // handle player 1
            while (Player1Controller is null)
            {
                Thread.Sleep(100);
            }

            PlayerTurn(Player1Controller);
            // handle player 2
            while (Player2Controller is null)
            {
                Thread.Sleep(100);
            }

            PlayerTurn(Player2Controller);
        }
    }

    private void NotifyStateChange()
    {
        Player1Controller.NotifyGameStateChanged(State);
        Player2Controller.NotifyGameStateChanged(State);
    }

    private void PlayerTurn(BaseController playerController)
    {
        // handle player
        _currentPlayer = playerController;
    }
}