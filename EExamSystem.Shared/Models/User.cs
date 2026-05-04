namespace EExamSystem.Shared.Models;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Student;
    
    public List<Course> InstructedCourses { get; set; } = new();
    
    public List<Section> EnrolledSections { get; set; } = new();
    
    public List<Exam> IndividuallyAssignedExams { get; set; } = new();
}