using System.Net.Http.Headers;

namespace EExamSystem.Client.Services;

/// <summary>
/// Scoped wrapper that creates HttpClients with the Bearer token already attached.
/// Must be scoped (not transient/singleton) so it shares the same circuit scope
/// as AuthService and ProtectedSessionStorage — avoids the IHttpClientFactory
/// handler scope mismatch on Blazor Server.
/// </summary>
public class ApiClient
{
    private readonly IHttpClientFactory _factory;
    private readonly IAuthService _authService;

    public ApiClient(IHttpClientFactory factory, IAuthService authService)
    {
        _factory = factory;
        _authService = authService;
    }

    public async Task<HttpClient> CreateAsync()
    {
        var client = _factory.CreateClient("ApiClient");
        var token = await _authService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
