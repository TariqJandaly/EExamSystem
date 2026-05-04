using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.Models;

public class QuestionOption
{
    public int Id { get; set; }

    /// <summary>
    /// The actual text of the choice (e.g., "4", "All of the above").
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Marks if this specific option is the correct answer. 
    /// </summary>
    public bool IsCorrect { get; set; }
    
    // Parent Relationship: Which question does this choice belong to?
    public int QuestionId { get; set; }
    public Question? Question { get; set; }
}