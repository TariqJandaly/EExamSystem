using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.DTOs.Chapters;

public class ChapterCreateDto
{
    [Required(ErrorMessage = "Chapter name is required.")]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;
}