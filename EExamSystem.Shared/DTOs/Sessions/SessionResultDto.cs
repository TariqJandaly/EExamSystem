namespace EExamSystem.Shared.DTOs.Sessions;

/// <summary>
/// A specialized container for passing session results from the service to the controller.
/// </summary>
public class SessionResultDto<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
}