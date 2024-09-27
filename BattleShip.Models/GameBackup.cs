namespace BattleShip.Models;

public class GameBackup(Player player, Grid grid)
{
    public Player Player { get; } = player;
    public Grid Grid { get; } = grid;
}