using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.Models;

public class Exam
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }

    public int DurationMinutes { get; set; }

    [Required]
    [Range(1, 1000)]
    public int MaxScore { get; set; } 

    [Required]
    public int PassingScore { get; set; } 
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public int CourseId { get; set; }
    public Course? Course { get; set; }

    public List<Section> AssignedSections { get; set; } = new();
    public List<User> AssignedStudents { get; set; } = new();
    public List<TestbankChapter> CoveredChapters { get; set; } = new();
}