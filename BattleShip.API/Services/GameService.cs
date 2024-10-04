namespace BattleShip.API.Services;

public class GameService
{
    public readonly PlayerManager PlayerManager;

    public readonly GameManager GameManager;

    public GameService()
    {
        PlayerManager = new PlayerManager();
        GameManager = new GameManager();
    }
}