namespace EExamSystem.Shared.DTOs.Testbanks;

public class UserRolesDto
{
    public string UserId { get; set; } = string.Empty;
    public List<string> Roles { get; set; }
}