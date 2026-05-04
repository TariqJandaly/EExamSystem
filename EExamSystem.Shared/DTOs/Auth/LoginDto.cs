using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.DTOs.Auth;

public class LoginDto
{
    [Required(ErrorMessage = "EmailRequired")]
    [EmailAddress(ErrorMessage = "EmailInvalidFormat")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "PasswordRequired")]
    [MinLength(8, ErrorMessage = "PasswordTooShort")]
    [DataType(DataType.Password, ErrorMessage = "PasswordInvalidFormat")]
    public string Password { get; set; } = string.Empty;
}