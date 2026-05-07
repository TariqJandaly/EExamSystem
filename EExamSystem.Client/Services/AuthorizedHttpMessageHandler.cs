using System.Net.Http.Headers;

namespace EExamSystem.Client.Services;

public class AuthorizedHttpMessageHandler : DelegatingHandler
{
    private readonly IAuthService _authService;

    public AuthorizedHttpMessageHandler(IAuthService authService)
    {
        _authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _authService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
