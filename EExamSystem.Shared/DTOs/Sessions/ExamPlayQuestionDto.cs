namespace EExamSystem.Shared.DTOs.Sessions;

/// <summary>
/// The safe version of a Question for the active exam interface.
/// </summary>
public class ExamPlayQuestionDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Points { get; set; }
    public List<ExamPlayOptionDto> Options { get; set; } = new();
}