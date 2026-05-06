namespace EExamSystem.Shared.DTOs.StudentDashboard;

/// <summary>
/// Detailed information displayed in the "Lobby" right before a student clicks "Start".
/// </summary>
public class ExamPreTestDetailsDto
{
    public int ExamId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int DurationMinutes { get; set; }
    public int MaxScore { get; set; }
    public int PassingScore { get; set; }
    public int TotalQuestions { get; set; }
}