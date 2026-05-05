namespace EExamSystem.Shared.DTOs.Questions;

public class OptionCreateDto
{
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}