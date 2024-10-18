using BattleShip.Models;
using Microsoft.AspNetCore.Components;

namespace BattleShip.App.Services;

public class LocalGameReplication
{
    public GameData? GameData { get; private set; }
    public PlayerData PlayerData { get; private set; } = new();
    public PlayerData OpponentData { get; private set; } = new();
    public GridData PlayerGrid { get; private set; } = new();
    public GridData OpponentGrid { get; private set; } = new();
    public bool IsTurn { get; private set; }

    private readonly NavigationManager _navigation;
    private readonly GameHub _gameHub;

    public GameState State { get; private set; } = GameState.WaitingForPlayers;
    
    public event Action<GameState>? OnStateChanged;
    public event Action<PlayerData>? OnPlayerUpdate;
    public event Action<GridData>? OnGridUpdate;
    public event Action<bool>? OnTurnChanged; 

    public LocalGameReplication(GameHub gameHub, NavigationManager navigation)
    {
        _gameHub = gameHub;
        _navigation = navigation;
        gameHub.OnGameJoined += OnGameJoined;
        gameHub.OnGameLeft += OnGameLeft;
        gameHub.OnGameStateChanged += OnStateChangedHandler;
        gameHub.OnPlayerUpdate += OnPlayerUpdateHandler;
        gameHub.OnGridUpdate += OnGridUpdatedHandler;
        gameHub.OnTurnChanged += OnTurnChangedHandler;
    }

    public void Attack(Coordinates coordinates)
    {
        _gameHub.SendAttack(GameData!.Id, coordinates);
    }

    private void OnGameJoined(GameData gameData)
    {
        GameData = gameData;
        _navigation.NavigateTo("/game");
    }
    
    private void OnGameLeft(GameData gameData)
    {
        GameData = null;
        // check if the player is in the game (/game)
        // if so, navigate to the menu
        string currentUri = _navigation.Uri;
        Console.WriteLine("Current URI: " + currentUri);
        _navigation.NavigateTo("/menu");
    }

    public void Refresh()
    {
        if (GameData is null)
        {
            return;
        }
        _gameHub.SendRefreshRequest();
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
    
    private void OnPlayerUpdateHandler(PlayerData data)
    {
        if (data.IsSelf)
        {
            PlayerData = data;
        }
        else
        {
            OpponentData = data;
        }
        OnPlayerUpdate?.Invoke(data);
    }
    
    private void OnGridUpdatedHandler(GridData data)
    {
        if (data.IsSelf)
        {
            PlayerGrid = data;
        }
        else
        {
            OpponentGrid = data;
        }
        OnGridUpdate?.Invoke(data);
    }
    
    private void OnTurnChangedHandler(bool isTurn)
    {
        IsTurn = isTurn;
        OnTurnChanged?.Invoke(isTurn);
    }
    
}