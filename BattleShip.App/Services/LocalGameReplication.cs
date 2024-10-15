using BattleShip.Models;
using Microsoft.AspNetCore.Components;

namespace BattleShip.App.Services;

public class LocalGameReplication
{
    public GameData? GameData { get; set; }

    private readonly NavigationManager _navigation;
    private readonly GameHub _gameHub;

    public GameState State { get; private set; } = GameState.WaitingForPlayers;
    
    public event Action<GameState>? OnStateChanged;

    public LocalGameReplication(GameHub gameHub, NavigationManager navigation)
    {
        _gameHub = gameHub;
        _navigation = navigation;
        gameHub.OnGameJoined += OnGameJoined;
        gameHub.OnGameLeft += OnGameLeft;
        gameHub.OnGameStateChanged += OnGameStateChanged;
    }
    
    private void OnGameJoined(GameData gameData)
    {
        GameData = gameData;
        _navigation.NavigateTo("/game");
    }
    
    private void OnGameLeft(GameData gameData)
    {
        GameData = null;
        _navigation.NavigateTo("/menu");
    }
    
    public void SendReady()
    {
        if (GameData is null)
        {
            return;
        }
        _gameHub.SendReady(GameData.Id);
    }
    
    private void OnGameStateChanged(GameState state)
    {
        OnStateChanged?.Invoke(state);
        State = state;
    }
    
}