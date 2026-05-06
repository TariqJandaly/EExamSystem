using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.DTOs.Sessions;

/// <summary>
/// Data required to auto-save a student's answer choice.
/// </summary>
public class SubmitAnswerDto
{
    [Required]
    public int QuestionId { get; set; }
    
    [Required]
    public int OptionId { get; set; }
}