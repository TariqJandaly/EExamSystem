namespace EExamSystem.Shared.Models;

public class Exam
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int DurationMinutes { get; set; }
    
    public int CourseId { get; set; }
    public Course? Course { get; set; }

    public List<Section> AssignedSections { get; set; } = new();
    public List<User> AssignedStudents { get; set; } = new();
    
    public List<TestbankChapter> CoveredChapters { get; set; } = new();
}