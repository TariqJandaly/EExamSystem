using EExamSystem.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Json;

namespace EExamSystem.Client.Services;

public class AuthService : IAuthService
{
    private const string TokenKey = "auth_token";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ProtectedSessionStorage _sessionStorage;
    private readonly AuthenticationStateProvider _authStateProvider;

    public AuthService(
        IHttpClientFactory httpClientFactory,
        ProtectedSessionStorage sessionStorage,
        AuthenticationStateProvider authStateProvider)
    {
        _httpClientFactory = httpClientFactory;
        _sessionStorage = sessionStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<AuthResponseDto> LoginAsync(string email, string password)
    {
        var http = _httpClientFactory.CreateClient("ApiClient");
        var loginDto = new LoginDto { Email = email, Password = password };

        HttpResponseMessage response;
        try
        {
            response = await http.PostAsJsonAsync("/api/v1/auth/login", loginDto);
        }
        catch
        {
            return new AuthResponseDto { IsSuccess = false, Message = "ServerError" };
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        if (result is null || !result.IsSuccess)
            return result ?? new AuthResponseDto { IsSuccess = false, Message = "UnknownError" };

        await _sessionStorage.SetAsync(TokenKey, result.Token);
        ((CustomAuthStateProvider)_authStateProvider).NotifyAuthStateChanged();
        return result;
    }

    public async Task LogoutAsync()
    {
        await _sessionStorage.DeleteAsync(TokenKey);
        ((CustomAuthStateProvider)_authStateProvider).NotifyAuthStateChanged();
    }

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            var result = await _sessionStorage.GetAsync<string>(TokenKey);
            return result.Success ? result.Value : null;
        }
        catch
        {
            return null;
        }
    }
}
