using BattleShip.Models;

namespace BattleShip.API.GameLogic;

public class PlayerController : BaseController
{
    
    public readonly Player Player;
    
    public PlayerController(Player player)
    {
        Player = player;
    }
    
    public void SetReady()
    {
        IsReady = true;
    }
    
}