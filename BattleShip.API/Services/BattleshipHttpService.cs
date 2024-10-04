using System.Security.Claims;
using BattleShip.Models;
using Microsoft.AspNetCore.Authorization;

namespace BattleShip.API.Services;

public class BattleshipHttpService
{
    
    private WebApplication _app = null!;
    
    public void RegisterRoutes(WebApplication app)
    {
        _app = app;
        
        _app.MapGet("/profile", [Authorize] async (HttpContext context) =>
        {
            return await Profile(context);
        }).WithName("Profile").WithOpenApi();
    }

    private async Task<IResult> Profile(HttpContext context)
    {
        ClaimsPrincipal user = context.User;
        string? token = ExtractToken(context);
        if (token is null) return Results.Unauthorized();

        Profile profile;
        try
        {
            profile = await _app.Services.GetRequiredService<AccountService>().GetUserProfile(user, token);
        }
        catch (Exception)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(profile);
    }
    
    private string? ExtractToken(HttpContext context)
    {
        string authHeader = context.Request.Headers["Authorization"].ToString();
        if (!authHeader.StartsWith("Bearer "))
        {
            return null;
        }

        return authHeader.Substring("Bearer ".Length).Trim();
    }
    
}