namespace EExamSystem.Shared.DTOs.Execution;

public class ExamResultDto
{
    public int SessionId { get; set; }
    public decimal FinalScore { get; set; }
    public int MaxScore { get; set; }
    public bool Passed { get; set; }
    public DateTime CompletedAt { get; set; }
}