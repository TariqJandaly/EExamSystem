using System.Security.Cryptography.X509Certificates;

namespace EExamSystem.Shared.DTOs.Auth;

public class AuthResponseDto
{
    public bool IsSuccess { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime Expiry { get; set; }

    /// <summary>
    /// Codes of errors that occurred during authentication
    /// This can be used by the client to determine the type of error and display an appropriate message to the user.
    /// </summary>
    public string Message { get; set; } = string.Empty;

}