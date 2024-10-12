using Microsoft.AspNetCore.SignalR;

namespace BattleShip.API.Services;

public class GameService : IDisposable
{

    public GameHubService GameHub => _serviceProvider.GetRequiredService<GameHubService>();
    
    private readonly IServiceProvider _serviceProvider;
    
    public readonly PlayerDatabase PlayerDatabase;

    public readonly SessionManager SessionManager;

    public readonly GameManager GameManager;

    public readonly QueueManager QueueManager;

    public GameService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        PlayerDatabase = new PlayerDatabase();
        SessionManager = new SessionManager();
        GameManager = new GameManager();
        QueueManager = new QueueManager(this);
    }

    public void Dispose()
    {
        QueueManager.Dispose();
    }
}