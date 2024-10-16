﻿using BattleShip.Models;
using BattleShip.API.GameLogic;
using Microsoft.AspNetCore.SignalR;

namespace BattleShip.API.Services;

public class GameHubService(IServiceProvider serviceProvider)
{
    
    private GameService GameService => serviceProvider.GetRequiredService<GameService>();
    private IHubContext<GameHub> HubContext => serviceProvider.GetRequiredService<IHubContext<GameHub>>();
    
    private IHubClients Clients => HubContext.Clients;
    
    public async Task NotifyJoinQueue(string playerId, QueueType queueType)
    {
        string connectionId = GameService.SessionManager.GetConnectionId(playerId)!;
        await Clients.Client(connectionId).SendAsync("NotifyJoinQueue", queueType);
    }
    
    public async Task NotifyLeaveQueue(string playerId)
    {
        if (!GameService.SessionManager.IsPlayerConnected(playerId))
        {
            return;
        }
        string connectionId = GameService.SessionManager.GetConnectionId(playerId)!;
        await Clients.Client(connectionId).SendAsync("NotifyLeaveQueue");
    }
    
    public async Task NotifyGameJoined(string playerId, Game game)
    {
        GameData gameDataData = GetGameData(game);
        string connectionId = GameService.SessionManager.GetConnectionId(playerId)!;
        await Clients.Client(connectionId).SendAsync("NotifyGameJoined", gameDataData);
    }
    
    public async Task NotifyGameLeft(string playerId, Game game)
    {
        Models.GameData gameDataData = GetGameData(game);
        string connectionId = GameService.SessionManager.GetConnectionId(playerId)!;
        await Clients.Client(connectionId).SendAsync("NotifyGameLeft", gameDataData);
    }

    public async Task NotifyGameStateChanged(string playerId, GameState gameState)
    {
        string connectionId = GameService.SessionManager.GetConnectionId(playerId)!;
        await Clients.Client(connectionId).SendAsync("NotifyGameStateChanged", gameState);
    }
    
    public async Task NotifyPlayerUpdate(string playerId, PlayerData data)
    {
        string connectionId = GameService.SessionManager.GetConnectionId(playerId)!;
        await Clients.Client(connectionId).SendAsync("NotifyPlayerUpdate", data);
    }
    
    public async Task NotifyGridUpdate(string playerId, GridData data)
    {
        string connectionId = GameService.SessionManager.GetConnectionId(playerId)!;
        await Clients.Client(connectionId).SendAsync("NotifyGridUpdate", data);
    }
    
    public async Task NotifyIsTurnChanged(string playerId, bool isTurn)
    {
        string connectionId = GameService.SessionManager.GetConnectionId(playerId)!;
        await Clients.Client(connectionId).SendAsync("NotifyIsTurnChanged", isTurn);
    }
    
    private GameData GetGameData(Game game)
    {
        return new GameData
        {
            Id = game.Id.ToString(),
            Settings = game.GameSettings
        };
    }
    
}