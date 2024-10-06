using Microsoft.AspNetCore.SignalR;

namespace BattleShip.API.Services;

public class GameHub : Hub
{
    
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
    
}