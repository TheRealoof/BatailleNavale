using BattleShip.API.Services;
using BattleShip.Models;

namespace BattleShip.API.GameLogic;

public class PlayerController : BaseController
{
    public readonly Player Player;

    private readonly string _name;
    private readonly string? _picture;
    
    public override string Name => _name;
    public override string? Picture => _picture;

    public PlayerController(Game game, PlayerGrid playerGrid, PlayerGrid opponentGrid, Player player) : base(game,
        playerGrid, opponentGrid)
    {
        Player = player;
        OnGameStateChanged += NotifyGameStateChanged;
        OnIsTurnChanged += IsTurnChanged;
        PlayerGrid.OnUpdate += NotifyPlayerGridUpdate;
        OpponentGrid.OnUpdate += NotifyOpponentGridUpdate;
        OnIsOpponentConnectedChanged += NotifyOpponentUpdate;
        Profile profile = GameService.ServiceProvider.GetRequiredService<AccountService>().GetUserProfile(Player.Id);
        _name = profile.UserName;
        _picture = profile.Picture;
    }

    public void SetReady()
    {
        IsReady = true;
    }

    private void NotifyGameStateChanged()
    {
        _ = GameService.GameHub.NotifyGameStateChanged(Player.Id, Game.State);
    }

    public void RefreshClient()
    {
        NotifyPlayerUpdate();
        NotifyOpponentUpdate();
        NotifyPlayerGridUpdate();
        NotifyOpponentGridUpdate();
    }

    private void NotifyPlayerUpdate()
    {
        PlayerData data = new PlayerData
        {
            IsSelf = true,
            IsConnected = IsConnected,
            Name = Name,
            Picture = Picture
        };
        _ = GameService.GameHub.NotifyPlayerUpdate(Player.Id, data);
    }

    private void NotifyOpponentUpdate()
    {
        PlayerData data = new PlayerData
        {
            IsSelf = false,
            IsConnected = Opponent.IsConnected,
            Name = Opponent.Name,
            Picture = Opponent.Picture
        };
        _ = GameService.GameHub.NotifyPlayerUpdate(Player.Id, data);
    }

    private void NotifyPlayerGridUpdate()
    {
        GridData data = new GridData()
        {
            IsSelf = true,
            ShipData = PlayerGrid.Ships
                .Select(ship => new ShipData
                {
                    Length = ship.Length,
                    Coordinates = ship.Coordinates,
                    Direction = ship.Direction
                })
                .ToList(),
            HitCoordinates = PlayerGrid.AttackedCoordinates
                .Where(PlayerGrid.IsShipPresent)
                .ToList(),
            MissCoordinates = PlayerGrid.AttackedCoordinates
                .Where(coordinates => !PlayerGrid.IsShipPresent(coordinates))
                .ToList()
        };
        _ = GameService.GameHub.NotifyGridUpdate(Player.Id, data);
    }

    private void NotifyOpponentGridUpdate()
    {
        GridData data = new GridData()
        {
            IsSelf = false,
            ShipData = OpponentGrid.SunkenShips
                .Where(ship => OpponentGrid.IsShipSunk(ship))
                .Select(ship => new ShipData
                {
                    Length = ship.Length,
                    Coordinates = ship.Coordinates,
                    Direction = ship.Direction
                })
                .ToList(),
            HitCoordinates = OpponentGrid.AttackedCoordinates
                .Where(OpponentGrid.IsShipPresent)
                .ToList(),
            MissCoordinates = OpponentGrid.AttackedCoordinates
                .Where(coordinates => !OpponentGrid.IsShipPresent(coordinates))
                .ToList()
        };
        _ = GameService.GameHub.NotifyGridUpdate(Player.Id, data);
    }

    private void IsTurnChanged()
    {
        _ = GameService.GameHub.NotifyIsTurnChanged(Player.Id, IsTurn);
    }

    public void InputAttack(Coordinates coordinates)
    {
        Attack(coordinates);
    }
}