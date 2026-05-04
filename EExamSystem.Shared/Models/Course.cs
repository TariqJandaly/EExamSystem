namespace EExamSystem.Shared.Models;

public class Course
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty; // e.g., CPCS202
    public string Name { get; set; } = string.Empty;

    public List<User> Instructors { get; set; } = new();
    public List<Testbank> Testbanks { get; set; } = new();
    public List<Section> Sections { get; set; } = new();
}