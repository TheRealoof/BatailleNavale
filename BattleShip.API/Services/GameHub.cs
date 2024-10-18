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
            Context.Abort();
            return;
        }

        string connectionId = Context.ConnectionId;
        string playerId = nameIdentifier.Value;

        Player player = gameService.PlayerDatabase.GetOrCreatePlayer(playerId);
        if (gameService.SessionManager.IsPlayerConnected(playerId))
        {
            Context.Abort();
            return;
        }
        gameService.SessionManager.PlayerConnected(connectionId, player);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string connectionId = Context.ConnectionId;
        gameService.SessionManager.PlayerDisconnected(connectionId);
        await base.OnDisconnectedAsync(exception);
    }
    
    public Task PlayerReady(string gameId)
    {
        string connectionId = Context.ConnectionId;
        Player player = gameService.SessionManager.GetPlayer(connectionId);

        GameLogic.Game? game = gameService.GameManager.GetGame(gameId);
        if (game is null)
        {
            return Task.CompletedTask;
        }
        
        gameService.PlayerControlManager.PlayerReady(player.Id);
        
        return Task.CompletedTask;
    }
    
    public Task PlayerPlaceShip(string gameId, ShipData shipData)
    {
        string connectionId = Context.ConnectionId;
        Player player = gameService.SessionManager.GetPlayer(connectionId);
        gameService.PlayerControlManager.PlayerPlaceShip(player.Id, shipData);
        return Task.CompletedTask;
    }
    
    public Task PlayerAttack(string gameId, Coordinates coordinates)
    {
        string connectionId = Context.ConnectionId;
        Player player = gameService.SessionManager.GetPlayer(connectionId);
        gameService.PlayerControlManager.PlayerAttack(player.Id, coordinates);
        return Task.CompletedTask;
    }

    public Task LeaveGame()
    {
        string connectionId = Context.ConnectionId;
        Player player = gameService.SessionManager.GetPlayer(connectionId);
        gameService.PlayerControlManager.PlayerLeave(player.Id);
        return Task.CompletedTask;
    }
    
    public Task RefreshClient()
    {
        string connectionId = Context.ConnectionId;
        Player player = gameService.SessionManager.GetPlayer(connectionId);
        gameService.PlayerControlManager.PlayerRefresh(player.Id);
        return Task.CompletedTask;
    }
    
}