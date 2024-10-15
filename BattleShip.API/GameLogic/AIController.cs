namespace BattleShip.API.GameLogic;

// ReSharper disable once InconsistentNaming
public class AIController : BaseController
{
    public AIController(Game game, PlayerGrid playerGrid, PlayerGrid opponentGrid) : base(game, playerGrid,
        opponentGrid)
    {
        IsReady = true;
    }
}