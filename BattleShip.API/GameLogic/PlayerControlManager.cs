namespace BattleShip.API.GameLogic;

public class PlayerControlManager
{
    
    private readonly Dictionary<string, PlayerController> _playerControllers = new();
    
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
    
}