using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.Models;

public class TestbankChapter
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public int TestbankId { get; set; }
    public Testbank? Testbank { get; set; }
    
    public List<Question> Questions { get; set; } = new();
    public List<Exam> CoveredInExams { get; set; } = new();
}