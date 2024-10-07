namespace BattleShip.API.Services;

public class GameService
{
    public readonly PlayerDatabase PlayerDatabase;
    
    public readonly SessionManager SessionManager;

    public readonly GameManager GameManager;

    public GameService()
    {
        PlayerDatabase = new PlayerDatabase();
        SessionManager = new SessionManager();
        GameManager = new GameManager();
    }
}