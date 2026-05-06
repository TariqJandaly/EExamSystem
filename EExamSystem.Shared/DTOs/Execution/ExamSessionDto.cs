namespace EExamSystem.Shared.DTOs.Execution;

public class ExamSessionDto
{
    public int SessionId { get; set; }
    public int ExamId { get; set; }
    public DateTime StartedAt { get; set; }
    public int DurationMinutes { get; set; }
}