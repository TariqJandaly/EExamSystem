using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.DTOs.Execution;

public class SubmitAnswerDto
{
    [Required]
    public int QuestionId { get; set; }
    
    [Required]
    public int SelectedOptionId { get; set; }
}