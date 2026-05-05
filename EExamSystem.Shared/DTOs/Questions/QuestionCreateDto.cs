using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.DTOs.Questions;

public class QuestionCreateDto
{
    [Required]
    public string Content { get; set; } = string.Empty;
    
    [Range(1, 100)]
    public int Points { get; set; }

    [Required]
    [MinLength(2, ErrorMessage = "A question must have at least 2 options.")]
    public List<OptionCreateDto> Options { get; set; } = new();
}