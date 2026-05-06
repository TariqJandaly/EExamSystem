namespace EExamSystem.Shared.DTOs.StudentDashboard;

/// <summary>
/// A specialized container for passing student dashboard results from the service to the controller.
/// </summary>
public class StudentExamsResultDto<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
}