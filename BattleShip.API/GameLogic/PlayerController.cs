using BattleShip.Models;

namespace BattleShip.API.GameLogic;

public class PlayerController : BaseController
{
    public readonly Player Player;

    public PlayerController(Game game, PlayerGrid playerGrid, PlayerGrid opponentGrid, Player player) : base(game,
        playerGrid, opponentGrid)
    {
        Player = player;
    }

    public void SetReady()
    {
        IsReady = true;
    }

    public override void NotifyGameStateChanged(GameState state)
    {
        _ = GameService.GameHub.NotifyGameStateChanged(Player.Id, state);
    }

    public override void NotifyPlayerUpdate()
    {
        GridData data = new GridData()
        {
            IsSelf = true,
            ShipData = PlayerGrid.Ships
                .Select(ship => new ShipData
                {
                    Length = ship.Length,
                    Coordinates = ship.Coordinates,
                    Direction = ship.Direction
                })
                .ToList(),
            HitCoordinates = PlayerGrid.AttackedCoordinates
                .Where(PlayerGrid.IsShipPresent)
                .ToList(),
            MissCoordinates = PlayerGrid.AttackedCoordinates
                .Where(coordinates => !PlayerGrid.IsShipPresent(coordinates))
                .ToList()
        };
        _ = GameService.GameHub.NotifyUpdate(Player.Id, data);
    }

    public override void NotifyOponentUpdate()
    {
        GridData data = new GridData()
        {
            IsSelf = false,
            ShipData = OpponentGrid.SunkenShips
                .Where(ship => OpponentGrid.IsShipSunk(ship))
                .Select(ship => new ShipData
                {
                    Length = ship.Length,
                    Coordinates = ship.Coordinates,
                    Direction = ship.Direction
                })
                .ToList(),
            HitCoordinates = OpponentGrid.AttackedCoordinates
                .Where(OpponentGrid.IsShipPresent)
                .ToList(),
            MissCoordinates = OpponentGrid.AttackedCoordinates
                .Where(coordinates => !OpponentGrid.IsShipPresent(coordinates))
                .ToList()
        };
        _ = GameService.GameHub.NotifyUpdate(Player.Id, data);
    }

    protected override void IsTurnChanged()
    {
        _ = GameService.GameHub.NotifyIsTurnChanged(Player.Id, IsTurn);
    }

    public void InputAttack(Coordinates coordinates)
    {
        Attack(coordinates);
        Console.WriteLine($"Player {Player.Id} attacked {coordinates}");
    }
}