namespace EExamSystem.Shared.Models;

public class StudentExamSession
{
    public int Id { get; set; }
    
    public int ExamId { get; set; }
    public Exam? Exam { get; set; }
    
    public int StudentId { get; set; }
    public User? Student { get; set; }
    
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal? FinalScore { get; set; }
    
    public List<StudentAnswer> Answers { get; set; } = new();
}