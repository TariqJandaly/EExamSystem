namespace EExamSystem.Shared.DTOs;

public class CourseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The official university course code. 
    /// Format: Letters followed by numbers with no spaces (e.g., "CPCS202").
    /// </summary>
    public string Code { get; set; } = string.Empty;
}