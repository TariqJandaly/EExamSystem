namespace EExamSystem.Shared.DTOs.Exams;

public class ExamDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int DurationMinutes { get; set; }
    public int MaxScore { get; set; }
    public int PassingScore { get; set; }
    public DateTime CreatedAt { get; set; }
}