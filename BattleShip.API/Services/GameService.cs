using Microsoft.AspNetCore.SignalR;
using BattleShip.API.GameLogic;

namespace BattleShip.API.Services;

public class GameService : IDisposable
{

    public GameHubService GameHub => ServiceProvider.GetRequiredService<GameHubService>();
    
    public readonly IServiceProvider ServiceProvider;
    
    public readonly PlayerDatabase PlayerDatabase;

    public readonly SessionManager SessionManager;

    public readonly GameManager GameManager;

    public readonly QueueManager QueueManager;
    
    public readonly PlayerControlManager PlayerControlManager;

    public GameService(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        PlayerDatabase = new PlayerDatabase();
        SessionManager = new SessionManager();
        GameManager = new GameManager();
        QueueManager = new QueueManager(this);
        PlayerControlManager = new PlayerControlManager(this);
    }

    public void Dispose()
    {
        QueueManager.Dispose();
        GameManager.Dispose();
    }
}