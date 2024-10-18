using BattleShip.API.Services;
using BattleShip.Models;

namespace BattleShip.API.GameLogic;

public class PlayerControlManager
{
    
    private readonly Dictionary<string, PlayerController> _playerControllers = new();
    
    public PlayerControlManager(GameService gameService)
    {
        gameService.SessionManager.OnPlayerDisconnected += player =>
        {
            PlayerLeave(player.Id);
        };
    }
    
    public void PlayerReady(string playerId)
    {
        PlayerController? playerController = GetPlayerController(playerId);
        playerController?.SetReady();
    }
    
    public void RegisterPlayerController(PlayerController playerController)
    {
        _playerControllers[playerController.Player.Id] = playerController;
    }
    
    public void UnregisterPlayerController(PlayerController playerController)
    {
        _playerControllers.Remove(playerController.Player.Id);
    }
    
    public PlayerController? GetPlayerController(string playerId)
    {
        _playerControllers.TryGetValue(playerId, out PlayerController? playerController);
        return playerController;
    }
    
    public void PlayerPlaceShip(string playerId, ShipData shipData)
    {
        PlayerController? playerController = GetPlayerController(playerId);
        playerController?.InputPlaceShip(shipData);
    }
    
    public void PlayerAttack(string playerId, Coordinates coordinates)
    {
        PlayerController? playerController = GetPlayerController(playerId);
        playerController?.InputAttack(coordinates);
    }
    
    public void PlayerLeave(string playerId)
    {
        PlayerController? playerController = GetPlayerController(playerId);
        if (playerController is null)
        {
            return;
        }
        playerController.IsConnected = false;
    }
    
    public void PlayerRefresh(string playerId)
    {
        PlayerController? playerController = GetPlayerController(playerId);
        playerController?.RefreshClient();
    }
    
}