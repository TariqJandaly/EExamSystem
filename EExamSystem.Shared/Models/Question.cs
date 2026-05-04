using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.Models;

public class Question
{
    public int Id { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// The weight of the question. Must be greater than 0.
    /// </summary>
    [Range(1, 100)]
    public int Points { get; set; } = 1;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public int TestbankChapterId { get; set; }
    public TestbankChapter? Chapter { get; set; }

    // ADDED: The list of A, B, C, D choices
    public List<QuestionOption> Options { get; set; } = new();
}
