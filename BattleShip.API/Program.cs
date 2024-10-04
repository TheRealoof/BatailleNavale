using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using Auth0.AspNetCore.Authentication;
using BattleShip.API;
using BattleShip.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin();
        policy.AllowAnyMethod();
        policy.AllowAnyHeader();
    });
});

builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"]!;
    options.ClientId = builder.Configuration["Auth0:ClientId"]!;
});

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth0:Authority"];
        options.Audience = builder.Configuration["Auth0:Audience"];
        //options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        //{
        //    ValidAudience = builder.Configuration["Auth0:Audience"],
        //    ValidIssuer = builder.Configuration["Auth0:Authority"]
        //};
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

app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapGet("/test", async (HttpRequest request) =>
{
    return new Profile()
    {
        UserName = "Test name",
        Email = "test@email.com"
    };
}).WithName("Test").WithOpenApi();

app.MapGet("/login", async (context) =>
{
    var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
        // Indicate here where Auth0 should redirect the user after a login.
        // Note that the resulting absolute Uri must be added to the
        // **Allowed Callback URLs** settings for the app.
        .WithRedirectUri("/")
        .Build();

    await context.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
}).WithName("Login").WithOpenApi();

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
}).WithName("Logout").WithOpenApi();

app.MapGet("/profile", [Authorize] (HttpContext context) =>
{
    Console.WriteLine("Profile endpoint hit");
    ClaimsPrincipal user = context.User;
    Console.WriteLine("User: " + user);
    IIdentity? identity = user.Identity;
    Console.WriteLine("Identity: " + identity + " Auth: " + identity?.IsAuthenticated);
    string? username = identity?.Name;
    Console.WriteLine("Username: " + username);

    // print all claims
    foreach (var claim in user.Claims)
    {
        Console.WriteLine(claim.Type + " : " + claim.Value);
    }
    
    //var nameIdentifier = user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
    var nameIdentifier = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    Console.WriteLine("NameIdentifier: " + nameIdentifier);
    
    var httpClient = new HttpClient();
    var authHeader = context.Request.Headers["Authorization"].ToString();
    if (!authHeader.StartsWith("Bearer "))
    {
        return Results.Unauthorized();
    }
    
    var token = authHeader.Substring("Bearer ".Length).Trim();
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    var response = httpClient.GetAsync("https://dev-xh5hwto5vb2xyc1m.us.auth0.com/userinfo").Result;
    
    if (!response.IsSuccessStatusCode)
        return Results.Unauthorized();
    
    var content = response.Content.ReadAsStringAsync().Result;
    Console.WriteLine("Content: " + content);
    
    return Results.Ok(new Profile
    {
        UserName = username,
        Email = null
    });
}).WithName("Profile").WithOpenApi();

app.MapPost("/exchange-code", async (HttpRequest request) =>
{
    // get code from request
    var code = request.Query["code"];
    // code to exchange code for token

    return code;
});

// register player by name returns player object
app.MapPost("/api/player", (GameService gameService, string name) => { gameService.PlayerManager.AddPlayer(name); })
    .WithName("CreatePlayer")
    .WithOpenApi();

// create new game returns the players boats and game id
app.MapPost("/api/game", (GameService gameService) => { gameService.GameManager.CreateGame(); })
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