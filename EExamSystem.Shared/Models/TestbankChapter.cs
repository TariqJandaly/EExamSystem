namespace EExamSystem.Shared.Models;

public class TestbankChapter
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public int TestbankId { get; set; }
    public Testbank? Testbank { get; set; }
    
    public List<Question> Questions { get; set; } = new();
    
    public List<Exam> CoveredInExams { get; set; } = new();
}