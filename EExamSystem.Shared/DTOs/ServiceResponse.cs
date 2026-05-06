namespace EExamSystem.Shared.DTOs;

/// <summary>
/// A generic response wrapper for service operations.
/// providing a consistent structure for success status, messages, and data payloads across the application.
/// </summary>
/// <typeparam name="T">Class type for the data payload</typeparam>
public class ServiceResponse<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; } = 200;
}