using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.DTOs.Users;

public class UserChangePasswordDto
{
    [Required(ErrorMessage = "NewPasswordRequired")]
    [MinLength(8, ErrorMessage = "NewPasswordTooShort")]
    [DataType(DataType.Password, ErrorMessage = "NewPasswordInvalidFormat")]
    public string newPassword { get; set; } = string.Empty;
}