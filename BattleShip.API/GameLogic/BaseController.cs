using BattleShip.API.Services;
using BattleShip.Models;

namespace BattleShip.API.GameLogic;

public abstract class BaseController
{
    public readonly Game Game;
    public GameService GameService => Game.GameService;
    public readonly PlayerGrid PlayerGrid;
    public readonly PlayerGrid OpponentGrid;
    public bool IsReady { get; protected set; }

    private bool _canPlaceShips;

    public bool CanPlaceShips
    {
        get => _canPlaceShips;
        set
        {
            _canPlaceShips = value;
            CanPlaceShipsChanged();
        }
    }
    
    private bool _isTurn;
    
    public bool IsTurn
    {
        get => _isTurn;
        set
        {
            _isTurn = value;
            IsTurnChanged();
        }
    }

    public BaseController(Game game, PlayerGrid playerGrid, PlayerGrid opponentGrid)
    {
        Game = game;
        PlayerGrid = playerGrid;
        OpponentGrid = opponentGrid;
        IsReady = false;
        CanPlaceShips = false;
        playerGrid.OnShipAdded += NotifyShipsChanged;
    }

    public virtual void NotifyGameStateChanged(GameState state)
    {
    }

    public virtual void NotifyShipsChanged()
    {
    }

    public virtual void CanPlaceShipsChanged()
    {
        if (!CanPlaceShips)
        {
            return;
        }

        for (var i = 0; i < PlayerGrid.ShipLengths.Length; i++)
        {
            int length = PlayerGrid.ShipLengths[i];
            GenerateShip(length);
        }
    }

    void GenerateShip(int length)
    {
        while (true)
        {
            Console.WriteLine("Attempting to place ship of length " + length);
            int x = new Random().Next(0, Game.GameSettings.GridWidth);
            int y = new Random().Next(0, Game.GameSettings.GridHeight);
            ShipDirection direction = (ShipDirection)new Random().Next(0, 4);
            Ship ship = new Ship(x, y, length, direction);
            
            if (IsShipValid(ship))
            {
                PlayerGrid.AddShip(ship);
                break;
            }
        }
    }
    
    private bool IsShipValid(Ship ship)
    {
        foreach (Coordinates coordinates in ship.CoordinatesList)
        {
            if (!PlayerGrid.IsInBounds(coordinates) || PlayerGrid.IsShipPresent(coordinates))
            {
                return false;
            }
        }
        return true;
    }
    
    public virtual void IsTurnChanged()
    {
    }
    
}