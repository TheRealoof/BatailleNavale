﻿using System.Net;
using System.Net.Http.Json;
using BattleShip.API.Protos;
using BattleShip.Models;
using Grpc.Core;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Profile = BattleShip.Models.Profile;
using QueueSettings = BattleShip.API.Protos.QueueSettings;

namespace BattleShip.App.Services;

public class GameServer(
    IAccessTokenProvider tokenProvider,
    HttpClient http,
    BattleshipService.BattleshipServiceClient grpcClient
)
{
    private static readonly bool UsingGrpc = false;

    public async Task<Profile?> GetProfile()
    {
        try
        {
            if (UsingGrpc)
            {
                var headers = new Metadata();
                await AddAuthorizationHeader(headers);
                var grpcProfile = await grpcClient.GetProfileAsync(new Empty(), headers);
                return new Profile
                {
                    UserName = grpcProfile.Username,
                    Picture = grpcProfile.Picture
                };
            }
            else
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "profile");
                await AddAuthorizationHeader(request);
                var response = await http.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to get profile");
                }

                return await response.Content.ReadFromJsonAsync<Profile>();
            }
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }

        return null;
    }

    public async void JoinQueue(Models.QueueSettings queueSettings)
    {
        if (UsingGrpc)
        {
            var headers = new Metadata();
            await AddAuthorizationHeader(headers);
            QueueSettings settings = new()
            {
                Type = queueSettings.Type,
                AiDifficulty = queueSettings.AIDifficulty
            };
            await grpcClient.JoinQueueAsync(settings, headers);
        }
        else
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "queue/join");
            await AddAuthorizationHeader(request);
            request.Content = JsonContent.Create(queueSettings);
            var response = await http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to join queue");
            }
        }
    }

    public async void LeaveQueue()
    {
        if (UsingGrpc)
        {
            var headers = new Metadata();
            await AddAuthorizationHeader(headers);
            await grpcClient.LeaveQueueAsync(new Empty(), headers);
        }
        else
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "queue/leave");
            await AddAuthorizationHeader(request);
            var response = await http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to leave queue");
            }
        }
    }

    private async Task AddAuthorizationHeader(HttpRequestMessage request)
    {
        var token = await GetAccessToken();
        request.Headers.Add("Authorization", $"Bearer {token}");
    }

    private async Task AddAuthorizationHeader(Metadata headers)
    {
        var token = await GetAccessToken();
        headers.Add("Authorization", $"Bearer {token}");
    }

    private async Task<string> GetAccessToken()
    {
        var tokenResult = await tokenProvider.RequestAccessToken();
        tokenResult.TryGetToken(out var token);
        return token!.Value;
    }
}