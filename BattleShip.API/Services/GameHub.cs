using System.Security.Claims;
using BattleShip.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BattleShip.API.Services;

[Authorize]
public class GameHub(GameService gameService) : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public override async Task OnConnectedAsync()
    {
        ClaimsPrincipal? claims = Context.User;

        Claim? nameIdentifier = claims?.FindFirst(ClaimTypes.NameIdentifier);
        if (nameIdentifier is null)
        {
            return;
        }

        string connectionId = Context.ConnectionId;
        string playerId = nameIdentifier.Value;

        Player player = gameService.PlayerDatabase.GetOrCreatePlayer(playerId);
        gameService.SessionManager.PlayerConnected(connectionId, player);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string connectionId = Context.ConnectionId;
        gameService.SessionManager.PlayerDisconnected(connectionId);

        await base.OnDisconnectedAsync(exception);
    }
    
}