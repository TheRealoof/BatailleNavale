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

    public override void NotifyShipsChanged()
    {
        _ = GameService.GameHub.NotifyShipsChanged(Player.Id, PlayerGrid.Ships.Select(ship => new ShipData
        {
            Length = ship.Length,
            Coordinates = ship.Coordinates,
            Direction = ship.Direction
        }).ToList());
    }

    public override void IsTurnChanged()
    {
        _ = GameService.GameHub.NotifyIsTurnChanged(Player.Id, IsTurn);
    }
}