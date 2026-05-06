using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.DTOs.Sections;

public class CreateSectionDto
{
    [Required(ErrorMessage = "Section name is required.")]
    [MaxLength(50, ErrorMessage = "Section name cannot exceed 50 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Course ID is required.")]
    public int CourseId { get; set; }
}