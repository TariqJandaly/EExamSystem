using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.DTOs.Questions;

public class OptionCreateDto
{
    [Required(AllowEmptyStrings = false)]
    [MaxLength(500)]
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}