using Microsoft.AspNetCore.Diagnostics;
namespace backend.src;

public class GlobalExceptionHandler(IHostEnvironment environment) : IExceptionHandler
{
    private readonly IHostEnvironment _environment = environment;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"An error occurred: {exception.Message}");

        var errorResponse = new
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            Message = "An unexpected error occurred. Please try again later.",
            StackTrace = ""
        };
        
        if (_environment.IsDevelopment())
        {
            errorResponse = errorResponse with 
            {
                Message = exception.Message,
                StackTrace = exception.StackTrace ?? ""
            };
        }

        httpContext.Response.StatusCode = errorResponse.StatusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);

        return true;
    }
}
