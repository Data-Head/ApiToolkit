using System.Net;

namespace ApiToolkit.ViewModels;

/**
 * API Response wrapper.
 * Defaults To - 
 * Status Code: 200,
 * Success: True,
 * Messages: Blank Array
 */
public class ApiResponse<T> where T : class
{
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

    public bool Success { get; set; } = true;

    public IList<string> Messages { get; set; } = new List<string>();

    public T? Data { get; set; } = null;

    public static ApiResponse<T> NotFound(string message = "Resource not found")
    {
        return new ApiResponse<T>()
        {
            Success = false,
            StatusCode = HttpStatusCode.NotFound,
            Messages = new List<string>() {message},
            Data = null
        };
    }

    public static ApiResponse<T> BadRequest(string message = "Bad Request", T? data = null)
    {
        return new ApiResponse<T>()
        {
            Success = false,
            StatusCode = HttpStatusCode.BadRequest,
            Messages = new List<string>() {message},
            Data = data
        };
    }

    public static ApiResponse<T> InternalServerError(string message = "Internal Server Error", T? data = null)
    {
        return new ApiResponse<T>()
        {
            Success = false,
            StatusCode = HttpStatusCode.InternalServerError,
            Messages = new List<string>() {message},
            Data = data
        };
    }
}