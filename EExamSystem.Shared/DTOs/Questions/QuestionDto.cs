namespace EExamSystem.Shared.DTOs.Questions;

public class QuestionDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Points { get; set; }
    
    public List<OptionDto> Options { get; set; } = new();
}