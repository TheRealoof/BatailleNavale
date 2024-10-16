using System.Security.Claims;
using BattleShip.API.Protos;
using BattleShip.Models;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Profile = BattleShip.Models.Profile;

namespace BattleShip.API.Services;

public class BattleshipHttpService(AccountService accountService, GameService gameService)
{
    
    public void RegisterRoutes(WebApplication app)
    {
        app.MapGet("/profile", [Authorize] async (HttpContext context) =>
        {
            return await Profile(context);
        }).WithName("Profile").WithOpenApi();
        
        app.MapPost("/queue/join", [Authorize] async (HttpContext context) =>
        {
            return await JoinQueue(context);
        }).WithName("JoinQueue").WithOpenApi();
        
        app.MapPost("/queue/leave", [Authorize] async (HttpContext context) =>
        {
            return await LeaveQueue(context);
        }).WithName("LeaveQueue").WithOpenApi();
    }

    private async Task<IResult> Profile(HttpContext context)
    {
        ClaimsPrincipal user = context.User;
        string? token = accountService.ExtractToken(context);
        if (token is null) return Results.Unauthorized();

        Profile profile;
        try
        {
            profile = await accountService.GetUserProfile(user, token);
        }
        catch (Exception)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(profile);
    }
    
    public async Task<IResult> JoinQueue(HttpContext context)
    {
        QueueSettings? request = await context.Request.ReadFromJsonAsync<QueueSettings>();
        if (!Enum.TryParse(request?.Type, out QueueType queueType))
        {
            return Results.BadRequest("Invalid queue type");
        }
        
        Player player = gameService.PlayerDatabase.GetOrCreatePlayer(GetUserId(context));
        gameService.QueueManager.JoinQueue(player, queueType);
        return Results.Ok();
    }
    
    public async Task<IResult> LeaveQueue(HttpContext context)
    {
        Player player = gameService.PlayerDatabase.GetOrCreatePlayer(GetUserId(context));
        gameService.QueueManager.LeaveQueue(player);
        return Results.Ok();
    }
    
    private ClaimsPrincipal GetClaims(HttpContext context)
    {
        return context.User;
    }
    
    private string GetUserId(HttpContext context)
    {
        return GetClaims(context).FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }
}