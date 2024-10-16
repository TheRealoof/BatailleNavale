using BattleShip.Models;
using Microsoft.AspNetCore.Components;

namespace BattleShip.App.Services;

public class LocalGameReplication
{
    public GameData? GameData { get; private set; }
    public List<ShipData> Ships { get; private set; } = new();

    private readonly NavigationManager _navigation;
    private readonly GameHub _gameHub;

    public GameState State { get; private set; } = GameState.WaitingForPlayers;
    
    public event Action<GameState>? OnStateChanged;
    public event Action<List<ShipData>>? OnShipsChanged; 

    public LocalGameReplication(GameHub gameHub, NavigationManager navigation)
    {
        _gameHub = gameHub;
        _navigation = navigation;
        gameHub.OnGameJoined += OnGameJoined;
        gameHub.OnGameLeft += OnGameLeft;
        gameHub.OnGameStateChanged += OnStateChangedHandler;
        gameHub.OnShipsChanged += OnShipsChangedHandler;
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
    
    private void OnStateChangedHandler(GameState state)
    {
        State = state;
        OnStateChanged?.Invoke(state);
    }
    
    private void OnShipsChangedHandler(List<ShipData> ships)
    {
        Ships = ships;
        OnShipsChanged?.Invoke(ships);
    }
    
}