﻿using Microsoft.AspNetCore.SignalR;
using BattleShip.API.GameLogic;

namespace BattleShip.API.Services;

public class GameService : IDisposable
{

    public GameHubService GameHub => _serviceProvider.GetRequiredService<GameHubService>();
    
    private readonly IServiceProvider _serviceProvider;
    
    public readonly PlayerDatabase PlayerDatabase;

    public readonly SessionManager SessionManager;

    public readonly GameManager GameManager;

    public readonly QueueManager QueueManager;
    
    public readonly PlayerControlManager PlayerControlManager;

    public GameService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        PlayerDatabase = new PlayerDatabase();
        SessionManager = new SessionManager();
        GameManager = new GameManager();
        QueueManager = new QueueManager(this);
        PlayerControlManager = new PlayerControlManager();
    }

    public void Dispose()
    {
        QueueManager.Dispose();
        GameManager.Dispose();
    }
}