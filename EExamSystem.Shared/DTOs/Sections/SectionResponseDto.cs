namespace EExamSystem.Shared.DTOs.Sections;

/// <summary>
/// A specialized DTO used to communicate the result of section operations from the service to the controller.
/// </summary>
public class SectionResultDto<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
}