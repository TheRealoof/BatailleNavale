using BattleShip.Models;
using Microsoft.AspNetCore.Components;

namespace BattleShip.App.Services;

public class LocalGameReplication
{
    public Game? Game { get; set; }

    private readonly NavigationManager _navigation;
    private readonly GameHub _gameHub;

    public LocalGameReplication(GameHub gameHub, NavigationManager navigation)
    {
        _gameHub = gameHub;
        _navigation = navigation;
        gameHub.OnGameJoined += OnGameJoined;
        gameHub.OnGameLeft += OnGameLeft;
    }
    
    private void OnGameJoined(Game game)
    {
        Game = game;
        _navigation.NavigateTo("/game");
    }
    
    private void OnGameLeft(Game game)
    {
        Game = null;
        _navigation.NavigateTo("/menu");
    }
    
    public void SendReady()
    {
        if (Game is null)
        {
            return;
        }
        _gameHub.SendReady(Game.Id);
    }
    
}