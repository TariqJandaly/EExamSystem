using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.Models;

public class Testbank
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    
    public List<TestbankChapter> Chapters { get; set; } = new();
}