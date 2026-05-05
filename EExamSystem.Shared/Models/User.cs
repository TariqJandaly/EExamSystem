using Microsoft.AspNetCore.Identity;

namespace EExamSystem.Shared.Models;

// <summary>
// Inherits from IdentityUser to integrate with ASP.NET Core Identity for authentication and authorization.
// </summary>
public class User : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    // public UserRole Role { get; set; } = UserRole.Student;

    public List<Course> InstructedCourses { get; set; } = new();

    public List<Section> EnrolledSections { get; set; } = new();

    public List<Exam> IndividuallyAssignedExams { get; set; } = new();
}