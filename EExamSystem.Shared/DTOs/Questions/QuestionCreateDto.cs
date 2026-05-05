using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.DTOs.Questions;

public class QuestionCreateDto : IValidatableObject
{
    [Required]
    public string Content { get; set; } = string.Empty;
    
    [Range(1, 100)]
    public int Points { get; set; }

    [Required]
    [MinLength(2, ErrorMessage = "A question must have at least 2 options.")]
    public List<OptionCreateDto> Options { get; set; } = new();
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var correctCount = Options.Count(o => o.IsCorrect);
        if (correctCount != 1)
        {
            yield return new ValidationResult(
                "Exactly one option must be marked as correct.",
                new[] { nameof(Options) });
        }
    }
}