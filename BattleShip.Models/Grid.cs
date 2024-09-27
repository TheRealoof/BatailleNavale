namespace BattleShip.Models;

public class Grid(uint width, uint height)
{
    
    public uint Width { get; } = width;
    public uint Height { get; } = height;
    public char[,] GridArray { get; } = new char[width, height];
}