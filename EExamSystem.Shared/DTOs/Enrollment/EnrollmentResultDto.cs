namespace EExamSystem.Shared.DTOs.Enrollment;

/// <summary>
/// A specialized container for passing enrollment operation results from the service to the controller.
/// </summary>
/// <typeparam name="T">The type of the result data.</typeparam>
public class EnrollmentResultDto<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
}