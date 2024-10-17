using System.Security.Claims;
using BattleShip.API.Protos;
using BattleShip.Models;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Profile = BattleShip.API.Protos.Profile;
using QueueSettings = BattleShip.Models.QueueSettings;

namespace BattleShip.API.Services;

// ReSharper disable once InconsistentNaming
public class BattleshipGRPCService(AccountService accountService, GameService gameService)
    : BattleshipService.BattleshipServiceBase
{
    [Authorize]
    public override async Task<Profile> GetProfile(Empty request, ServerCallContext context)
    {
        ClaimsPrincipal claims = GetClaims(context);
        string? token = GetToken(context);

        if (token is null)
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid token"));
        }

        Models.Profile profile;
        try
        {
            profile = await accountService.GetUserProfile(claims, token);
        }
        catch (Exception)
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid token"));
        }

        return new Profile
        {
            Username = profile.UserName,
            Picture = profile.Picture
        };
    }

    [Authorize]
    public override Task<Empty> JoinQueue(Protos.QueueSettings request, ServerCallContext context)
    {
        if (!Enum.TryParse(request.Type, out QueueType queueType))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid queue type"));
        }
        
        Player player = gameService.PlayerDatabase.GetOrCreatePlayer(GetUserId(context));
        gameService.QueueManager.JoinQueue(player, new QueueSettings
        {
            Type = queueType.ToString(),
            AIDifficulty = null
        });
        return Task.FromResult(new Empty());
    }

    [Authorize]
    public override Task<Empty> LeaveQueue(Empty request, ServerCallContext context)
    {
        Player player = gameService.PlayerDatabase.GetOrCreatePlayer(GetUserId(context));
        gameService.QueueManager.LeaveQueue(player);
        return Task.FromResult(new Empty());
    }

    private ClaimsPrincipal GetClaims(ServerCallContext context)
    {
        return context.GetHttpContext().User;
    }
    
    private string GetUserId(ServerCallContext context)
    {
        return GetClaims(context).FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }
    
    private string? GetToken(ServerCallContext context)
    {
        return accountService.ExtractToken(context.GetHttpContext());
    }
}