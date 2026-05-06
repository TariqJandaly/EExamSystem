using System.ComponentModel.DataAnnotations;

namespace EExamSystem.Shared.DTOs.Testbanks;

public class TestbankCreateDto
{
    [Required(ErrorMessage = "TestbankNameRequired")]
    [MaxLength(150, ErrorMessage = "NameLengthLimitExceeded")]
    public string Name { get; set; }

    [Required(ErrorMessage = "CourseRequired")]
    public int CourseId { get; set; }
}