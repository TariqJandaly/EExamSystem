using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.DTOs.Exams;

public class ExamCreateDto
{
    [Required(ErrorMessage = "Exam title is required.")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Required]
    [Range(0, 600)]
    public int DurationMinutes { get; set; }

    [Required]
    [Range(1, 1000, ErrorMessage = "Max score must be greater than 0.")]
    public int MaxScore { get; set; }

    [Required]
    [Range(0, 1000, ErrorMessage = "Passing score must be valid.")]
    public int PassingScore { get; set; }
}