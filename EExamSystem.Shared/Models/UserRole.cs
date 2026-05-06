namespace EExamSystem.Shared.Models;

/// <summary>
/// Defines the authorization level of a user across the entire platform.
/// </summary>
public static class UserRole
{
    public const string Chair = "CHAIR";
    public const string Admin = "ADMIN";
    public const string Instructor = "INSTRUCTOR";
    public const string Student = "STUDENT";
}