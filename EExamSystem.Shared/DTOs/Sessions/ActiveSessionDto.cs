namespace EExamSystem.Shared.DTOs.Sessions;

/// <summary>
/// The payload delivered to the frontend when a student starts or resumes a test.
/// </summary>
public class ActiveSessionDto
{
    public int SessionId { get; set; }
    public int ExamId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    
    /// <summary>
    /// The absolute deadline for this specific student's session (StartedAt + DurationMinutes).
    /// </summary>
    public DateTime SessionDeadline { get; set; }
    
    public List<ExamPlayQuestionDto> Questions { get; set; } = new();
}