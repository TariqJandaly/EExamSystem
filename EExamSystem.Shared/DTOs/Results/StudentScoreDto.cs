namespace EExamSystem.Shared.DTOs.Results;

/// <summary>
/// Represents a single student's performance on an exam.
/// </summary>
public class StudentScoreDto
{
    public string StudentId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>
    /// Will be null if the student has not yet submitted or started the exam.
    /// </summary>
    public decimal? FinalScore { get; set; }
    
    /// <summary>
    /// Will be null if the student has not yet submitted the exam.
    /// </summary>
    public DateTime? CompletedAt { get; set; }
}