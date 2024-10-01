using System.Security.Claims;
using Auth0.AspNetCore.Authentication;
using BattleShip.API;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"]!;
    options.ClientId = builder.Configuration["Auth0:ClientId"]!;
});
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<GameService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapPost("/login", async (context) =>
{
    var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
        // Indicate here where Auth0 should redirect the user after a login.
        // Note that the resulting absolute Uri must be added to the
        // **Allowed Callback URLs** settings for the app.
        .WithRedirectUri("/")
        .Build();

    await context.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
});

app.MapPost("/logout", [Authorize] async (context) =>
{
    var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
        // Indicate here where Auth0 should redirect the user after a logout.
        // Note that the resulting absolute Uri must be added to the
        // **Allowed Logout URLs** settings for the app.
        .WithRedirectUri("/")
        .Build();

    await context.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
});

app.MapPost("/profile", [Authorize](context) =>
{
    var user = context.User;
    var claims = user.Claims.Select(c => new { c.Type, c.Value });
    string? userName = user.Identity?.Name;
    string? userEmail = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
    Results.Ok(new { userName, userEmail });
    return Task.CompletedTask;
});

app.MapPost("/exchange-code", async (HttpRequest request) =>
{
    // get code from request
    var code = request.Query["code"];
    // code to exchange code for token
    
    return code;
});

// register player by name returns player object
app.MapPost("/api/player", (GameService gameService, string name) =>
    {
        gameService.PlayerManager.AddPlayer(name);
    })
    .WithName("CreatePlayer")
    .WithOpenApi();

// create new game returns the players boats and game id
app.MapPost("/api/game", (GameService gameService) =>
    {
        gameService.GameManager.CreateGame();
    })
    .WithName("CreateGame")
    .WithOpenApi();

// attack (gameId, x, y) returns game state (winner if game is over , hit or miss and oponnent's attack otherwise)
app.MapPost("/api/game/{gameId}/attack",
        ([FromRoute] string gameId, string playerId, GameService gameService, uint x, uint y) =>
        {
            gameService.GameManager.PerformAttack(gameId, playerId, x, y);
        })
    .WithName("Attack")
    .WithOpenApi();

app.Run();