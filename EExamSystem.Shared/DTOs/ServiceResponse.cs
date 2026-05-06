using System.ComponentModel;

namespace EExamSystem.Shared.DTOs;

/// <summary>
/// Base response for operations that don't return data.
/// </summary>
public class ServiceResponse
{
    [DefaultValue(true)]
    public bool Success { get; set; } = true;
    
    public string Message { get; set; } = string.Empty;
    
    public int StatusCode { get; set; }
}

/// <summary>
/// A generic response wrapper for operations that return a payload.
/// </summary>
public class ServiceResponse<T> : ServiceResponse
{
    public T? Data { get; set; }
}

/// <summary>
/// Error response for operations that don't return data.
/// Indicates that an operation failed with Success set to false by default.
/// </summary>
public class ErrorServiceResponse : ServiceResponse
{
    [DefaultValue(false)]
    public new bool Success { get; set; } = false;
}

/// <summary>
/// Generic error response for operations that return a payload on failure.
/// Indicates that an operation failed with Success set to false by default, while still providing error-related data.
/// </summary>
/// <typeparam name="T">The type of error data being returned.</typeparam>
public class ErrorServiceResponse<T> : ServiceResponse
{
    [DefaultValue(false)]
    public new bool Success { get; set; } = false;
    
    public T? Data { get; set; }
}