using BattleShip.Models;
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
        string connectionId = GameService.SessionManager.GetConnectionId(playerId)!;
        await Clients.Client(connectionId).SendAsync("NotifyLeaveQueue");
    }
    
    public async Task NotifyGameJoined(string playerId, GameLogic.Game game)
    {
        Console.WriteLine($"NotifyGameJoined: {game.Id.ToString()}");
        Models.Game gameData = GetGameData(game);
        string connectionId = GameService.SessionManager.GetConnectionId(playerId)!;
        await Clients.Client(connectionId).SendAsync("NotifyGameJoined", gameData);
    }
    
    public async Task NotifyGameLeft(string playerId, GameLogic.Game game)
    {
        Models.Game gameData = GetGameData(game);
        string connectionId = GameService.SessionManager.GetConnectionId(playerId)!;
        await Clients.Client(connectionId).SendAsync("NotifyGameLeft", gameData);
    }
    
    private Models.Game GetGameData(GameLogic.Game game)
    {
        return new Models.Game
        {
            Id = game.Id.ToString(),
            Settings = game.GameSettings
        };
    }
    
}