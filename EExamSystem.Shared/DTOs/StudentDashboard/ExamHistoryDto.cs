namespace EExamSystem.Shared.DTOs.StudentDashboard;

/// <summary>
/// Represents a completed exam session for the student's history view.
/// </summary>
public class ExamHistoryDto
{
    public int SessionId { get; set; }
    public int ExamId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal? FinalScore { get; set; }
    public int MaxScore { get; set; }
}