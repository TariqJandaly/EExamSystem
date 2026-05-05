using System.Security.Cryptography.X509Certificates;

namespace EExamSystem.Shared.DTOs.Auth;

public class AuthResponseDto
{
    public bool IsSuccess { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime Expiry { get; set; }
    public string Message { get; set; } = string.Empty;

}