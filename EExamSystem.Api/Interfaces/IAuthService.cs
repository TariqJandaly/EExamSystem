using EExamSystem.Shared.DTOs.Auth;

namespace EExamSystem.Api.Interfaces;

// Auth service interface
public interface IAuthService
{
    // For register oprations
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);

    // For login oprations
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);

}