﻿using BattleShip.API.Services;
using BattleShip.Models;

namespace BattleShip.API.GameLogic;

public abstract class BaseController
{
    public readonly Game Game;
    public GameService GameService => Game.GameService;
    public readonly PlayerGrid PlayerGrid;
    public readonly PlayerGrid OpponentGrid;
    public bool IsReady { get; protected set; }
    
    private bool _isConnected;
    
    public bool IsConnected
    {
        get => _isConnected;
        set
        {
            _isConnected = value;
            OnIsConnectedChanged?.Invoke();
        }
    }
    
    private bool _isOpponentConnected;
    
    public bool IsOpponentConnected
    {
        get => _isOpponentConnected;
        set
        {
            _isOpponentConnected = value;
            OnIsOpponentConnectedChanged?.Invoke();
        }
    }

    private bool _canPlaceShips;

    public bool CanPlaceShips
    {
        get => _canPlaceShips;
        set
        {
            _canPlaceShips = value;
            OnCanPlaceShipsChanged?.Invoke();
        }
    }
    
    private bool _isTurn;
    
    public bool IsTurn
    {
        get => _isTurn;
        set
        {
            _isTurn = value;
            OnIsTurnChanged?.Invoke();
        }
    }
    
    public event Action? OnIsConnectedChanged;
    public event Action? OnIsOpponentConnectedChanged;
    public event Action? OnCanPlaceShipsChanged;
    public event Action? OnIsTurnChanged;

    public BaseController(Game game, PlayerGrid playerGrid, PlayerGrid opponentGrid)
    {
        Game = game;
        PlayerGrid = playerGrid;
        OpponentGrid = opponentGrid;
        IsReady = false;
        IsConnected = true;
        CanPlaceShips = false;
        playerGrid.OnUpdate += NotifyPlayerUpdate;
        opponentGrid.OnUpdate += NotifyOpponentUpdate;
        OnCanPlaceShipsChanged += CanPlaceShipsChanged;
    }
    
    protected void Attack(Coordinates coordinates)
    {
        if (!IsTurn)
        {
            return;
        }
        if (!OpponentGrid.CanAttack(coordinates))
        {
            return;
        }
        OpponentGrid.Attack(coordinates);
        IsTurn = false;
    }

    public virtual void NotifyGameStateChanged(GameState state)
    {
    }

    public virtual void NotifyPlayerUpdate()
    {
    }
    
    public virtual void NotifyOpponentUpdate()
    {
    }

    public void CanPlaceShipsChanged()
    {
        if (!CanPlaceShips)
        {
            return;
        }

        for (var i = 0; i < PlayerGrid.ShipLengths.Length; i++)
        {
            int length = PlayerGrid.ShipLengths[i];
            GenerateShip(length);
        }
    }

    void GenerateShip(int length)
    {
        while (true)
        {
            Console.WriteLine("Attempting to place ship of length " + length);
            int x = new Random().Next(0, Game.GameSettings.GridWidth);
            int y = new Random().Next(0, Game.GameSettings.GridHeight);
            ShipDirection direction = (ShipDirection)new Random().Next(0, 4);
            Ship ship = new Ship(x, y, length, direction);
            
            if (IsShipValid(ship))
            {
                PlayerGrid.AddShip(ship);
                break;
            }
        }
    }
    
    private bool IsShipValid(Ship ship)
    {
        foreach (Coordinates coordinates in ship.CoordinatesList)
        {
            if (!PlayerGrid.IsInBounds(coordinates) || PlayerGrid.IsShipPresent(coordinates))
            {
                return false;
            }
        }
        return true;
    }
    
}