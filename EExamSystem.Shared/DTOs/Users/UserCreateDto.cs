using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.DTOs.Users;

public class UserCreateDto
{
    [Required(ErrorMessage = "FullNameRequired")]
    [MaxLength(128, ErrorMessage = "FullNameExceededMaxLength")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "EmailRequired")]
    [EmailAddress(ErrorMessage = "EmailInvalidFormat")]
    [MaxLength(128, ErrorMessage = "EmailExceededMaxLength")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "PasswordRequired")]
    [MinLength(8, ErrorMessage = "PasswordTooShort")]
    [DataType(DataType.Password, ErrorMessage = "PasswordInvalidFormat")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "RolesRequired")]
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}