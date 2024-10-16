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
    public event Action<List<ShipData>>? OnShipsChanged; 

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
        
        HubConnection.On<List<ShipData>>("NotifyShipsChanged", ships =>
        {
            Console.WriteLine("Ships changed");
            OnShipsChanged?.Invoke(ships);
        });
    }
    
    public void SendReady(string gameId)
    {
        if (HubConnection is null)
        {
            return;
        }
        HubConnection.SendAsync("PlayerReady", gameId);
    }
    
}