using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The BCrypt hashed version of the user's password. Never store plain text.
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;
    
    public UserRole Role { get; set; } = UserRole.Student;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public List<Course> InstructedCourses { get; set; } = new();
    public List<Section> EnrolledSections { get; set; } = new();
    public List<Exam> IndividuallyAssignedExams { get; set; } = new();
}