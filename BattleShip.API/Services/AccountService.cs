using System.Net.Http.Headers;
using System.Security.Claims;
using BattleShip.Models;

namespace BattleShip.API.Services;

public class AccountService
{
    private readonly Dictionary<string, Profile> _profiles = new();

    private readonly Profile _defaultProfile = new()
    {
        UserName = "Guest",
        Picture = null
    };

    public Profile GetUserProfile(string id)
    {
        return _profiles.GetValueOrDefault(id, _defaultProfile);
    }

    public async Task<Profile> GetUserProfile(string id, string token)
    {
        if (_profiles.TryGetValue(id, out var foundProfile))
        {
            return foundProfile;
        }

        var httpClient = new HttpClient();

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = httpClient.GetAsync("https://dev-xh5hwto5vb2xyc1m.us.auth0.com/userinfo").Result;

        if (!response.IsSuccessStatusCode)
        {
            return _defaultProfile;
        }

        var content = (await response.Content.ReadFromJsonAsync<Dictionary<string, object>>())!;

        Profile profile = new Profile
        {
            UserName = content["nickname"].ToString(),
            Picture = content["picture"].ToString()
        };
        _profiles.Add(id, profile);

        return profile;
    }

    public string? ExtractToken(HttpContext context)
    {
        string authHeader = context.Request.Headers["Authorization"].ToString();
        if (!authHeader.StartsWith("Bearer "))
        {
            return null;
        }

        return authHeader.Substring("Bearer ".Length).Trim();
    }
}