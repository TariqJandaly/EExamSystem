namespace EExamSystem.Shared.DTOs.Testbanks;

public class TestbankDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The ID of the course this testbank belongs to.
    /// </summary>
    public int CourseId { get; set; }

}