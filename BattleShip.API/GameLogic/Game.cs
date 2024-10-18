using BattleShip.API.Services;
using BattleShip.Models;

namespace BattleShip.API.GameLogic;

public class Game : IDisposable
{
    public readonly GameService GameService;

    public readonly Guid Id;

    public readonly GameSettings GameSettings;

    public GameState State { get; private set; }

    private BaseController _player1Controller = null!;
    public BaseController Player1Controller
    {
        get => _player1Controller;
        set
        {
            _player1Controller = value;
            OnPlayer1Set();
        }
    }
    private BaseController _player2Controller = null!;
    public BaseController Player2Controller
    {
        get => _player2Controller;
        set
        {
            _player2Controller = value;
            OnPlayer2Set();
        }
    }

    public readonly PlayerGrid Player1Grid;
    public readonly PlayerGrid Player2Grid;

    private bool _isRunning;
    private readonly Thread _gameThread;

    public Action? OnStop;

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
        if (_isRunning) return;
        State = GameState.WaitingForPlayers;
        _isRunning = true;
        _gameThread.Start();
    }

    public void Stop()
    {
        if (!_isRunning) return;
        _isRunning = false;
        _gameThread.Join();
        OnStop?.Invoke();
    }

    public void Dispose()
    {
        Stop();
    }

    private void RunGame()
    {
        if (Player1Controller is null || Player2Controller is null)
        {
            throw new InvalidOperationException("Both players must be set before starting the game.");
        }

        Player1Controller.CanPlaceShips = false;
        Player2Controller.CanPlaceShips = false;
        Player1Controller.IsTurn = false;
        Player2Controller.IsTurn = false;

        State = GameState.WaitingForPlayers;
        NotifyStateChange();
        WaitForPlayers();

        if (!_isRunning) return;

        State = GameState.PlacingShips;
        NotifyStateChange();
        PlaceShips();

        if (!_isRunning) return;

        State = GameState.Playing;
        NotifyStateChange();
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
    }

    private void GameLogic()
    {
        while (_isRunning)
        {
            PlayerTurn(Player1Controller);
            CheckWin();
            if (State == GameState.GameOver)
            {
                break;
            }

            PlayerTurn(Player2Controller);
            CheckWin();
            if (State == GameState.GameOver)
            {
                break;
            }
        }
    }

    private void CheckWin()
    {
        if (Player1Grid.SunkenShips.Count == Player1Grid.Ships.Count ||
            Player2Grid.SunkenShips.Count == Player2Grid.Ships.Count)
        {
            State = GameState.GameOver;
        }
    }

    private void NotifyStateChange()
    {
        Player1Controller.GameStateChanged();
        Player2Controller.GameStateChanged();
    }

    private void PlayerTurn(BaseController playerController)
    {
        playerController.IsTurn = true;
        while (_isRunning && playerController.IsTurn)
        {
            Thread.Sleep(100);
        }
    }
    
    private void OnPlayer1Set()
    {
        Player1Controller.OnIsConnectedChanged += IsPlayer1ConnectedChanged;
    }
    
    private void OnPlayer2Set()
    {
        Player2Controller.OnIsConnectedChanged += IsPlayer2ConnectedChanged;
    }

    private void IsPlayer1ConnectedChanged()
    {
        CheckDisconnect();
        if (Player2Controller != null!)
        {
            Player2Controller.IsOpponentConnected = Player1Controller.IsConnected;
        }
    }
    
    private void IsPlayer2ConnectedChanged()
    {
        CheckDisconnect();
        if (Player1Controller != null!)
        {
            Player1Controller.IsOpponentConnected = Player2Controller.IsConnected;
        }
    }

    private void CheckDisconnect()
    {
        if (!_isRunning) return;
        if (!Player1Controller.IsConnected && !Player2Controller.IsConnected)
        {
            Stop();
        }
    }
    
}