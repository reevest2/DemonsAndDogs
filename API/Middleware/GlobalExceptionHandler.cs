using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var traceId = httpContext.TraceIdentifier;

        logger.LogError(exception,
            "Unhandled {ExceptionType} at {RequestPath} [TraceId: {TraceId}]",
            exception.GetType().Name,
            httpContext.Request.Path,
            traceId);

        var problemDetails = new ProblemDetails
        {
            Title = "Internal Server Error",
            Detail = exception.Message,
            Status = StatusCodes.Status500InternalServerError,
            Extensions = { ["traceId"] = traceId }
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
