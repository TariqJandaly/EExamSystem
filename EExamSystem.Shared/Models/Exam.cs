using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.Models;

public class Exam
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// The exact UTC time the exam becomes available to students.
    /// </summary>
    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }

    /// <summary>
    /// The total time allowed for the exam in minutes. A value of 0 indicates an untimed exam.
    /// </summary>
    public int DurationMinutes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public int CourseId { get; set; }
    public Course? Course { get; set; }

    public List<Section> AssignedSections { get; set; } = new();
    public List<User> AssignedStudents { get; set; } = new();
    public List<TestbankChapter> CoveredChapters { get; set; } = new();
}