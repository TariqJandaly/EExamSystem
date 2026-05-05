using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.DTOs.Exams;

public class ExamCreateDto
{
    [Required(ErrorMessage = "Exam title is required.")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Required]
    [Range(1, 600, ErrorMessage = "Time limit must be between 1 and 600 minutes.")]
    public int TimeLimitInMinutes { get; set; }

    [Required]
    [Range(0, 100, ErrorMessage = "Passing score must be a percentage between 0 and 100.")]
    public int PassingScore { get; set; }
}