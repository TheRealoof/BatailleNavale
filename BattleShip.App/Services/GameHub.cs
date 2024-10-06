using Microsoft.AspNetCore.SignalR.Client;

namespace BattleShip.App.Services;

public class GameHub
{
    public HubConnection HubConnection { get; private set; }

    public event Action? OnStateChanged;

    public GameHub()
    {
        HubConnection = new HubConnectionBuilder()
            .WithUrl(new Uri("https://localhost:5001/gamehub"))
            .WithAutomaticReconnect()
            .Build();

        HubConnection.Closed += (_) =>
        {
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
        if (HubConnection.State == HubConnectionState.Disconnected)
        {
            await HubConnection.StartAsync();
            OnStateChanged?.Invoke();
        }
    }

    public async Task StopConnectionAsync()
    {
        if (HubConnection.State == HubConnectionState.Connected)
        {
            await HubConnection.StopAsync();
            OnStateChanged?.Invoke();
        }
    }
}