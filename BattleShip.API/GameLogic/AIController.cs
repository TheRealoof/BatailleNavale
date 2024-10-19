using BattleShip.Models;

namespace BattleShip.API.GameLogic;

// ReSharper disable once InconsistentNaming
public class AIController : BaseController
{
    public override string Name => "AI";
    public override string? Picture => null;

    public readonly AIDifficulty Difficulty;

    // ReSharper disable once InconsistentNaming
    public enum AIDifficulty
    {
        Easy = 0,
        Hard = 1
    }

    public AIController(Game game, PlayerGrid playerGrid, PlayerGrid opponentGrid, AIDifficulty aiDifficulty)
        : base(game, playerGrid, opponentGrid)
    {
        Difficulty = aiDifficulty;
        IsReady = true;
        OnCanPlaceShipsChanged += CanPlaceShipsChanged;
        OnIsTurnChanged += IsTurnChanged;
        OnIsOpponentConnectedChanged += IsOpponentConnectedChanged;
    }

    private void IsTurnChanged()
    {
        if (!IsTurn)
        {
            return;
        }

        Play();
    }

    private void IsOpponentConnectedChanged()
    {
        // Auto disconnect if opponent is disconnected so that the game can end
        if (!IsOpponentConnected)
        {
            IsConnected = false;
        }
    }

    private void CanPlaceShipsChanged()
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

    private void GenerateShip(int length)
    {
        while (true)
        {
            Coordinates coordinates = new()
            {
                X = new Random().Next(0, Game.GameSettings.GridWidth),
                Y = new Random().Next(0, Game.GameSettings.GridHeight)
            };
            ShipDirection direction = (ShipDirection)new Random().Next(0, 4);
            Ship ship = new Ship(coordinates, length, direction);

            if (IsShipValid(ship))
            {
                PlaceShip(ship);
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

    private void Play()
    {
        switch (Difficulty)
        {
            case AIDifficulty.Easy:
                PlayEasy();
                break;
            case AIDifficulty.Hard:
                PlayHard();
                break;
        }
    }

    private void AttackRandom()
    {
        while (true)
        {
            Coordinates coordinates = new()
            {
                X = new Random().Next(0, OpponentGrid.Width),
                Y = new Random().Next(0, OpponentGrid.Height)
            };
            if (!OpponentGrid.CanAttack(coordinates)) continue;
            Attack(coordinates);
            break;
        }
    }

    private void PlayEasy()
    {
        AttackRandom();
    }

    private void PlayHard()
    {
        List<Coordinates> currentTargets = [..OpponentGrid.AttackedCoordinates.Where(OpponentGrid.IsShipPresent)];
        foreach (Ship ship in OpponentGrid.SunkenShips)
        {
            foreach (Coordinates coordinates in ship.CoordinatesList)
            {
                currentTargets.Remove(coordinates); // Don't target sunken ships
            }
        }

        if (currentTargets.Count == 0)
        {
            AttackRandom();
            return;
        }

        Coordinates[] directions =
        [
            new() { X = 1, Y = 0 },
            new() { X = -1, Y = 0 },
            new() { X = 0, Y = 1 },
            new() { X = 0, Y = -1 }
        ];
        while (true)
        {
            Coordinates target = currentTargets[new Random().Next(0, currentTargets.Count)];
            Coordinates direction = directions[new Random().Next(0, directions.Length)];
            Coordinates coordinates = target + direction;
            if (!OpponentGrid.CanAttack(coordinates)) continue;
            Attack(coordinates);
            break;
        }
    }

}