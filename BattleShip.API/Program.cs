using BattleShip.API;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<GameService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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