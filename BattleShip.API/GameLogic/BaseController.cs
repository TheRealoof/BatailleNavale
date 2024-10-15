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

    public BaseController(Game game, PlayerGrid playerGrid, PlayerGrid opponentGrid)
    {
        Game = game;
        PlayerGrid = playerGrid;
        OpponentGrid = opponentGrid;
        IsReady = false;
        CanPlaceShips = false;
    }

    public virtual void NotifyGameStateChanged(GameState state)
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
        bool success = false;
        while (!success)
        {
            int x = new Random().Next(0, Game.GameSettings.GridWidth);
            int y = new Random().Next(0, Game.GameSettings.GridHeight);
            BoatDirection direction = (BoatDirection) new Random().Next(0, 4);
            Ship ship = new Ship(x, y, length, direction);
            // TODO check if ship doesn't overlap with other ships
            PlayerGrid.AddShip(ship);
            success = true;
        }
    }
    
}