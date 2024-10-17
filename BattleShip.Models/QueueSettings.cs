namespace BattleShip.Models;

public class QueueSettings
{
    public string Type { get; init; } = "";
    
    // ReSharper disable once InconsistentNaming
    public int? AIDifficulty { get; init; }
}