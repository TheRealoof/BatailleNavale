using BattleShip.Models;

namespace BattleShip.API.GameLogic;

// ReSharper disable once InconsistentNaming
public class AIController : BaseController
{
    public AIController(Game game, PlayerGrid playerGrid, PlayerGrid opponentGrid) : base(game, playerGrid,
        opponentGrid)
    {
        IsReady = true;
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
    
    public override string Name => "AI";
    public override string? Picture => null;
    
}