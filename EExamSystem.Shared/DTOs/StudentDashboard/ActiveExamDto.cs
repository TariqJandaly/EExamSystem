namespace EExamSystem.Shared.DTOs.StudentDashboard;

/// <summary>
/// Represents an exam that is currently available or upcoming for the student.
/// </summary>
public class ActiveExamDto
{
    public int ExamId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int DurationMinutes { get; set; }
    public int MaxScore { get; set; }
}