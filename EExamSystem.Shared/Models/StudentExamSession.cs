namespace EExamSystem.Shared.Models;

public class StudentExamSession
{
    public int Id { get; set; }
    
    public int ExamId { get; set; }
    public Exam? Exam { get; set; }
    
    public int StudentId { get; set; }
    public User? Student { get; set; }
    
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// Will remain null until the student explicitly submits the exam or time expires.
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Will remain null until the session is completed and graded.
    /// </summary>
    public decimal? FinalScore { get; set; }
    
    public List<StudentAnswer> Answers { get; set; } = new();
}