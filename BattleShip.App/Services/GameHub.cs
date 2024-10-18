using BattleShip.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;

namespace BattleShip.App.Services;

public class GameHub(IAccessTokenProvider tokenProvider)
{
    public HubConnection? HubConnection { get; private set; }
    
    public event Action? OnStateChanged;
    public event Action<QueueType>? OnQueueJoined;
    public event Action? OnQueueLeft;
    public event Action<GameData>? OnGameJoined;
    public event Action<GameData>? OnGameLeft;
    public event Action<GameState>? OnGameStateChanged;
    public event Action<PlayerData>? OnPlayerUpdate;
    public event Action<GridData>? OnGridUpdate;
    public event Action<bool>? OnTurnChanged;

    private async Task BuildHubConnection()
    {
        AccessTokenResult token = await tokenProvider.RequestAccessToken();
        token.TryGetToken(out AccessToken? accessToken);
        if (accessToken is null)
        {
            return;
        }
        HubConnection = new HubConnectionBuilder()
            .WithUrl(new Uri("https://localhost:5001/gamehub"), options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(accessToken.Value)!;
            })
            .Build();

        HubConnection.Closed += (_) =>
        {
            HubConnection = null;
            OnStateChanged?.Invoke();
            return Task.CompletedTask;
        };
        HubConnection.Reconnected += (_) =>
        {
            OnStateChanged?.Invoke();
            return Task.CompletedTask;
        };
        HubConnection.Reconnecting += (_) =>
        {
            OnStateChanged?.Invoke();
            return Task.CompletedTask;
        };
    }
    
    public async Task StartConnectionAsync()
    {
        if (HubConnection is null)
        {
            await BuildHubConnection();
        }
        if (HubConnection is null)
        {
            return;
        }
        if (HubConnection.State == HubConnectionState.Disconnected)
        {
            try
            {
                await HubConnection.StartAsync();
                OnStateChanged?.Invoke();
                ListenToMessages();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public async Task StopConnectionAsync()
    {
        if (HubConnection is not null && HubConnection.State == HubConnectionState.Connected)
        {
            await HubConnection.StopAsync();
            HubConnection = null;
            OnStateChanged?.Invoke();
        }
    }
    
    private void ListenToMessages()
    {
        if (HubConnection is null)
        {
            return;
        }
        
        Console.WriteLine("Listening to messages");
        
        HubConnection.On<QueueType>("NotifyJoinQueue", type =>
        {
            OnQueueJoined?.Invoke(type);
        });
        
        HubConnection.On("NotifyLeaveQueue", () =>
        {
            OnQueueLeft?.Invoke();
        });
        
        HubConnection.On<GameData>("NotifyGameJoined", game =>
        {
            OnGameJoined?.Invoke(game);
        });
        
        HubConnection.On<GameData>("NotifyGameLeft", game =>
        {
            OnGameLeft?.Invoke(game);
        });
        
        HubConnection.On<GameState>("NotifyGameStateChanged", state =>
        {
            OnGameStateChanged?.Invoke(state);
        });
        
        HubConnection.On<PlayerData>("NotifyPlayerUpdate", data =>
        {
            OnPlayerUpdate?.Invoke(data);
        });
        
        HubConnection.On<GridData>("NotifyGridUpdate", data =>
        {
            OnGridUpdate?.Invoke(data);
        });
        
        HubConnection.On<bool>("NotifyIsTurnChanged", isTurn =>
        {
            OnTurnChanged?.Invoke(isTurn);
        });
    }
    
    public void SendReady(string gameId)
    {
        HubConnection?.SendAsync("PlayerReady", gameId);
    }
    
    public void SendAttack(string gameId, Coordinates coordinates)
    {
        HubConnection?.SendAsync("PlayerAttack", gameId, coordinates);
    }

    public void SendLeaveGame()
    {
        HubConnection?.SendAsync("LeaveGame");
    }

    public void SendRefreshRequest()
    {
        HubConnection?.SendAsync("RefreshClient");
    }
    
}