namespace EExamSystem.Shared.DTOs.Sections;

public class SectionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CourseId { get; set; }
}