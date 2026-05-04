namespace EExamSystem.Shared.Models;

public class Section
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public int CourseId { get; set; }
    public Course? Course { get; set; }

    public List<User> Students { get; set; } = new();
    public List<Exam> AssignedExams { get; set; } = new();
}