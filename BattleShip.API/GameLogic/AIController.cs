using BattleShip.Models;

namespace BattleShip.API.GameLogic;

// ReSharper disable once InconsistentNaming
public class AIController : BaseController
{
    
    public override string Name => "AI";
    public override string? Picture => null;
    
    public AIController(Game game, PlayerGrid playerGrid, PlayerGrid opponentGrid) : base(game, playerGrid,
        opponentGrid)
    {
        IsReady = true;
        OnCanPlaceShipsChanged += CanPlaceShipsChanged;
        OnIsTurnChanged += IsTurnChanged;
        OnIsOpponentConnectedChanged += IsOpponentConnectedChanged;
    }

    private void IsTurnChanged()
    {
        if (!IsTurn)
        {
            return;
        }
        Play();
    }
    
    private void IsOpponentConnectedChanged()
    {
        // Auto disconnect if opponent is disconnected so that the game can end
        if (!IsOpponentConnected)
        {
            IsConnected = false;
        }
    }
    
    private void CanPlaceShipsChanged()
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

    private void GenerateShip(int length)
    {
        while (true)
        {
            Coordinates coordinates = new()
            {
                X = new Random().Next(0, Game.GameSettings.GridWidth),
                Y = new Random().Next(0, Game.GameSettings.GridHeight)
            };
            ShipDirection direction = (ShipDirection)new Random().Next(0, 4);
            Ship ship = new Ship(coordinates, length, direction);
            
            if (IsShipValid(ship))
            {
                PlaceShip(ship);
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

    private void Play()
    {
        while (true)
        {
            Coordinates coordinates = new()
            {
                X = new Random().Next(0, OpponentGrid.Width),
                Y = new Random().Next(0, OpponentGrid.Height)
            };
            if (!OpponentGrid.CanAttack(coordinates)) continue;
            Attack(coordinates);
            break;
        }
    }
    
}