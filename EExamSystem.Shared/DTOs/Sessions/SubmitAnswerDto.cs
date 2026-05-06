using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.DTOs.Sessions;

/// <summary>
/// Data required to auto-save a student's answer choice.
/// </summary>
public class SubmitAnswerDto
{
    [Range(1, int.MaxValue, ErrorMessage = "A valid QuestionId must be provided.")]
    public int QuestionId { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "A valid OptionId must be provided.")]
    public int OptionId { get; set; }
}