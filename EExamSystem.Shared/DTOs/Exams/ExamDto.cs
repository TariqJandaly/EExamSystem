namespace EExamSystem.Shared.DTOs.Exams;

public class ExamDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int TimeLimitInMinutes { get; set; }
    public int PassingScore { get; set; }
}