using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.Models;

public class StudentAnswer
{
    public int Id { get; set; }
    
    public int SessionId { get; set; }
    public StudentExamSession? Session { get; set; }
    
    public int QuestionId { get; set; }
    public Question? Question { get; set; }
    
    public int? SelectedOptionId { get; set; }
    public QuestionOption? SelectedOption { get; set; }

    /// <summary>
    /// Calculated field. True if the SelectedOption's IsCorrect property is true.
    /// </summary>
    public bool IsCorrect { get; set; }
}