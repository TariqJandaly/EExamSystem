namespace EExamSystem.Shared.DTOs.Assignments;

/// <summary>
/// A specialized container for passing assignment operation results from the service to the controller.
/// </summary>
/// <typeparam name="T">The type of the result data.</typeparam>
public class AssignmentResultDto<T>
{
    /// <summary>
    /// Indicates if the internal assignment logic was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// The resulting data from the operation.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// A code or message describing the result (e.g., "CourseMismatch", "AlreadyAssigned").
    /// </summary>
    public string Message { get; set; } = string.Empty;
}