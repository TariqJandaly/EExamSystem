using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.Models;

public class Course
{
    public int Id { get; set; }

    /// <summary>
    /// The official university course code. 
    /// Format: Letters followed by numbers with no spaces (e.g., "CPCS202").
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<User> Instructors { get; set; } = new();
    public List<Testbank> Testbanks { get; set; } = new();
    public List<Section> Sections { get; set; } = new();
}