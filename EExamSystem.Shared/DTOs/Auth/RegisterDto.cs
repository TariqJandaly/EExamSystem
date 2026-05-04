using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.DTOs.Auth;

public class RegisterDto
{
    [Required(ErrorMessage = "EmailRequired")]
    [EmailAddress(ErrorMessage = "EmailInvalidFormat")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "FullNameRequired")]
    [MaxLength(128, ErrorMessage = "FullNameExceededMaxLength")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "PasswordRequired")]
    [MinLength(8, ErrorMessage = "PasswordTooShort")]
    [DataType(DataType.Password, ErrorMessage = "PasswordInvalidFormat")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "ConfirmPasswordRequired")]
    [MinLength(8, ErrorMessage = "ConfirmPasswordTooShort")]
    [DataType(DataType.Password, ErrorMessage = "ConfirmPasswordInvalidFormat")]
    [Compare("Password", ErrorMessage = "PasswordsDoNotMatch")]
    public string ConfirmPassword { get; set; } = string.Empty;

}