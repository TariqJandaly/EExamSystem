namespace EExamSystem.Shared.DTOs.Users;

public class UserRolesDto
{
    public string UserId { get; set; } = string.Empty;
    public List<string> Roles { get; set; }
}