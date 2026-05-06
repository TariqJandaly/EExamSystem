namespace EExamSystem.Shared.DTOs.Results;

/// <summary>
/// A comprehensive grading report for a specific section taking a specific exam.
/// </summary>
public class SectionResultsDto
{
    public int ExamId { get; set; }
    public string ExamTitle { get; set; } = string.Empty;
    public int MaxScore { get; set; }
    public int PassingScore { get; set; }
    
    public string SectionName { get; set; } = string.Empty;
    
    public List<StudentScoreDto> StudentScores { get; set; } = new();
}