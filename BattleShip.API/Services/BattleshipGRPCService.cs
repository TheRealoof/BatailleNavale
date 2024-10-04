using Grpc.Core;

namespace BattleShip.API.Services;

// ReSharper disable once InconsistentNaming
public class BattleshipGRPCService : BattleshipService.BattleshipServiceBase
{

    public readonly GameService GameService;
    
    public BattleshipGRPCService(GameService gameService)
    {
        this.GameService = gameService;
    }
    
    public override Task<AttackResponseGRPC> Attack(AttackRequestGRPC request, ServerCallContext context)
    {
        return base.Attack(request, context);
    }
}