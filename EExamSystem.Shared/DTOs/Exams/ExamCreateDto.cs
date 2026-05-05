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
    [Range(0, 600, ErrorMessage = "Duration must be between 0 and 600 minutes.")]
    public int DurationMinutes { get; set; }
}