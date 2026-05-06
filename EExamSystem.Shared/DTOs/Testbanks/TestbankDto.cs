namespace EExamSystem.Shared.DTOs.Testbanks;

public class TestbankDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CourseId { get; set; }

}