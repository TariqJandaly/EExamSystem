namespace EExamSystem.Shared.DTOs.Sessions;

/// <summary>
/// The safe version of an Option that does NOT reveal if it is correct.
/// </summary>
public class ExamPlayOptionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
}