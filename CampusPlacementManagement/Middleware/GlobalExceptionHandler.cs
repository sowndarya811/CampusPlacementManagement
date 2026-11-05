
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An error occurred.");

        var statusCode = exception switch
        {
            ArgumentException => 400,
            KeyNotFoundException => 404,
            UnauthorizedAccessException => 403,
            InvalidOperationException => 409,
            _ => 500
        };

        var response = new ProblemDetails
        {
            Status = statusCode,
            Title = "Request Error",
            Detail = statusCode == 500
                ? "An unexpected server error occurred."
                : exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            response, cancellationToken);

        return true;
    }
}
