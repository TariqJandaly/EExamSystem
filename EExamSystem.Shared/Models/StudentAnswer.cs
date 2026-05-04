namespace EExamSystem.Shared.Models;

public class StudentAnswer
{
    public int Id { get; set; }
    
    public int SessionId { get; set; }
    public StudentExamSession? Session { get; set; }
    
    public int QuestionId { get; set; }
    public Question? Question { get; set; }
    
    public string SelectedAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}