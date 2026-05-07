namespace EExamSystem.Shared.DTOs.Results;

/// <summary>
/// A specialized container for passing result operations from the service to the controller.
/// </summary>
public class GradingResultDto<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
}

