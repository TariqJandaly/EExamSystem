using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace EExamSystem.Client.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private const string TokenKey = "auth_token";
    private readonly ProtectedSessionStorage _sessionStorage;

    public CustomAuthStateProvider(ProtectedSessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var result = await _sessionStorage.GetAsync<string>(TokenKey);
            if (!result.Success || string.IsNullOrWhiteSpace(result.Value))
                return Anonymous();

            if (IsTokenExpired(result.Value))
            {
                await _sessionStorage.DeleteAsync(TokenKey);
                return Anonymous();
            }

            var claims = ParseClaimsFromJwt(result.Value);
            var identity = new ClaimsIdentity(claims, "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch
        {
            // ProtectedSessionStorage throws during SSR pre-render — return anonymous safely
            return Anonymous();
        }
    }

    public void NotifyAuthStateChanged()
        => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    private static AuthenticationState Anonymous()
        => new(new ClaimsPrincipal(new ClaimsIdentity()));

    private static bool IsTokenExpired(string token)
    {
        var payload = ParsePayload(token);
        if (payload.TryGetValue("exp", out var expElement))
        {
            var expiry = DateTimeOffset.FromUnixTimeSeconds(expElement.GetInt64());
            return expiry < DateTimeOffset.UtcNow;
        }
        return true;
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string token)
    {
        var payload = ParsePayload(token);
        var claims = new List<Claim>();

        foreach (var kvp in payload)
        {
            var element = kvp.Value;

            // The API writes new Claim("role", role) which becomes "role" in the JWT payload.
            // .NET's [Authorize(Roles=...)] checks ClaimTypes.Role — must remap explicitly.
            if (kvp.Key == "role")
            {
                if (element.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in element.EnumerateArray())
                        claims.Add(new Claim(ClaimTypes.Role, item.GetString() ?? ""));
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, element.GetString() ?? ""));
                }
                continue;
            }

            // JwtSecurityTokenHandler shortens ClaimTypes.NameIdentifier to "nameid"
            // and ClaimTypes.Email to "email" in the outbound JWT payload.
            var claimType = kvp.Key switch
            {
                "nameid" => ClaimTypes.NameIdentifier,
                "email"  => ClaimTypes.Email,
                "FullName" => "FullName",
                _ => kvp.Key
            };

            if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in element.EnumerateArray())
                    claims.Add(new Claim(claimType, item.GetString() ?? ""));
            }
            else if (element.ValueKind != JsonValueKind.Null &&
                     element.ValueKind != JsonValueKind.Object)
            {
                claims.Add(new Claim(claimType, element.ToString()));
            }
        }

        return claims;
    }

    private static Dictionary<string, JsonElement> ParsePayload(string token)
    {
        var parts = token.Split('.');
        if (parts.Length != 3)
            return [];

        var payload = parts[1]
            .Replace('-', '+')
            .Replace('_', '/');

        payload = (payload.Length % 4) switch
        {
            2 => payload + "==",
            3 => payload + "=",
            _ => payload
        };

        var bytes = Convert.FromBase64String(payload);
        var json = Encoding.UTF8.GetString(bytes);
        return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json) ?? [];
    }
}
