using System.Security.Claims;
using BattleShip.API.Protos;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace BattleShip.API.Services;

// ReSharper disable once InconsistentNaming
public class BattleshipGRPCService(AccountService accountService, GameService gameService)
    : BattleshipService.BattleshipServiceBase
{
    [Authorize]
    public override async Task<Profile> GetProfile(Empty request, ServerCallContext context)
    {
        ClaimsPrincipal claims = context.GetHttpContext().User;
        string? token = accountService.ExtractToken(context.GetHttpContext());

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
}