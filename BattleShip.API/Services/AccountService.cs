using System.Net.Http.Headers;
using System.Security.Claims;
using BattleShip.Models;

namespace BattleShip.API.Services;

public class AccountService
{
    
    private readonly Dictionary<string, Profile> _profiles = new();
    
    public async Task<Profile> GetUserProfile(ClaimsPrincipal claims, string token)
    {
        if (_profiles.TryGetValue(token, out var foundProfile))
        {
            return foundProfile;
        }
        
        var nameIdentifier = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine("NameIdentifier: " + nameIdentifier);

        var httpClient = new HttpClient();

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = httpClient.GetAsync("https://dev-xh5hwto5vb2xyc1m.us.auth0.com/userinfo").Result;

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception();
        }
        
        var content = (await response.Content.ReadFromJsonAsync<Dictionary<string, object>>())!;
        
        Profile profile = new Profile
        {
            UserName = content["nickname"].ToString(),
            Picture = content["picture"].ToString()
        };
        _profiles.Add(token, profile);

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