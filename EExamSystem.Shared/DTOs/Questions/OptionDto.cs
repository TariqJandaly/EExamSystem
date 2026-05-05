namespace EExamSystem.Shared.DTOs.Questions;

public class OptionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}