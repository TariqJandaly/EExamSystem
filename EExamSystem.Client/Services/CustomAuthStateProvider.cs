using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace EExamSystem.Client.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    // TODO: read JWT from storage, parse claims, expose auth state
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        return Task.FromResult(new AuthenticationState(anonymous));
    }
}
