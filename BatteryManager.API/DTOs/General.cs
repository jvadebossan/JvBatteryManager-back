namespace BatteryManager.API.DTOs;

public class ApiResponse<T>
{
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public object? Errors { get; set; }
    public string? Code { get; set; }

    public static ApiResponse<T> SuccessResponse(string message, T? data = default)
        => new ApiResponse<T> { Message = message, Data = data };

    public static ApiResponse<T> ErrorResponse(string message, object? errors = null, string? code = null)
        => new ApiResponse<T> { Message = message, Errors = errors, Code = code };
}