using BattleShip.Models;

namespace BattleShip.API.GameLogic;

// ReSharper disable once InconsistentNaming
public class AIController : BaseController
{
    public AIController(Game game, PlayerGrid playerGrid, PlayerGrid opponentGrid) : base(game, playerGrid,
        opponentGrid)
    {
        IsReady = true;
    }

    protected override void IsTurnChanged()
    {
        if (!IsTurn)
        {
            return;
        }
        Play();
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