using System.Security.Claims;
using BattleShip.Models;
using Microsoft.AspNetCore.Authorization;

namespace BattleShip.API.Services;

public class BattleshipHttpService(AccountService accountService)
{
    
    public void RegisterRoutes(WebApplication app)
    {
        app.MapGet("/profile", [Authorize] async (HttpContext context) =>
        {
            return await Profile(context);
        }).WithName("Profile").WithOpenApi();
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
    
}