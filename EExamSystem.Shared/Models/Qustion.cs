namespace EExamSystem.Shared.Models;

public class Question
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public int Points { get; set; } = 1;
    
    public int TestbankChapterId { get; set; }
    public TestbankChapter? Chapter { get; set; }
}