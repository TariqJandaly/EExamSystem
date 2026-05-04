namespace EExamSystem.Shared.Models;

public class Testbank
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    
    public List<TestbankChapter> Chapters { get; set; } = new();
}