using System.Security.Cryptography.X509Certificates;

namespace EExamSystem.Shared.DTOs.Auth;

public class AuthResponseDto
{
    public bool IsSuccess { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public string message { get; set; } = string.Empty;

}