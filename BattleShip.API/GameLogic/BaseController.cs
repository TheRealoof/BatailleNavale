using BattleShip.API.Services;
using BattleShip.Models;

namespace BattleShip.API.GameLogic;

public abstract class BaseController
{
    public readonly Game Game;
    public GameService GameService => Game.GameService;
    public readonly PlayerGrid PlayerGrid;
    public readonly PlayerGrid OpponentGrid;
    public BaseController Opponent => Game.Player1Controller == this ? Game.Player2Controller : Game.Player1Controller;
    public bool IsReady { get; protected set; }
    
    private bool _isConnected;
    
    public bool IsConnected
    {
        get => _isConnected;
        set
        {
            _isConnected = value;
            OnIsConnectedChanged?.Invoke();
        }
    }
    
    private bool _isOpponentConnected;
    
    public bool IsOpponentConnected
    {
        get => _isOpponentConnected;
        set
        {
            _isOpponentConnected = value;
            OnIsOpponentConnectedChanged?.Invoke();
        }
    }

    private bool _canPlaceShips;

    public bool CanPlaceShips
    {
        get => _canPlaceShips;
        set
        {
            _canPlaceShips = value;
            OnCanPlaceShipsChanged?.Invoke();
        }
    }
    
    private bool _isTurn;
    
    public bool IsTurn
    {
        get => _isTurn;
        set
        {
            _isTurn = value;
            OnIsTurnChanged?.Invoke();
        }
    }
    
    public event Action? OnGameStateChanged;
    public event Action? OnIsConnectedChanged;
    public event Action? OnIsOpponentConnectedChanged;
    public event Action? OnCanPlaceShipsChanged;
    public event Action? OnIsTurnChanged;

    public BaseController(Game game, PlayerGrid playerGrid, PlayerGrid opponentGrid)
    {
        Game = game;
        PlayerGrid = playerGrid;
        OpponentGrid = opponentGrid;
        IsReady = false;
        IsConnected = true;
        CanPlaceShips = false;
    }

    protected void PlaceShip(Ship ship)
    {
        if (!CanPlaceShips)
        {
            return;
        }
        PlayerGrid.AddShip(ship);
    }
    
    protected void Attack(Coordinates coordinates)
    {
        if (!IsTurn)
        {
            return;
        }
        if (!OpponentGrid.CanAttack(coordinates))
        {
            return;
        }
        OpponentGrid.Attack(coordinates);
        IsTurn = false;
    }
    
    public void GameStateChanged()
    {
        OnGameStateChanged?.Invoke();
    }
    
    public abstract string Name { get; }
    public abstract string? Picture { get; }
    
}