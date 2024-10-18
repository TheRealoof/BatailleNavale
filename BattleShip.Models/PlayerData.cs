namespace BattleShip.Models;

public class PlayerData
{
    public bool IsSelf { get; set; } = false;
    public bool IsConnected { get; set; } = true;
    public string Name { get; set; } = string.Empty;
    public string? Picture { get; set; } = null;
}