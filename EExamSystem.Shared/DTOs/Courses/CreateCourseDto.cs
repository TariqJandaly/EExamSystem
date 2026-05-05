using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.DTOs.Courses;

public class CreateCourseDto
{
    [Required, MinLength(1)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The official university course code. 
    /// Format: Letters followed by numbers with no spaces (e.g., "CPCS202").
    /// </summary>
    [Required, RegularExpression(@"^[A-Za-z]+\d+$")]
    public string Code { get; set; } = string.Empty;
}