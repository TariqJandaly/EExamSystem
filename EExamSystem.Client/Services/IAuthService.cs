using EExamSystem.Shared.DTOs.Auth;

namespace EExamSystem.Client.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(string email, string password);
    Task LogoutAsync();
    Task<string?> GetTokenAsync();
}
