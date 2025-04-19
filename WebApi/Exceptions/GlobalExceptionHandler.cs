using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Exceptions
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An exception occurred: {Message}", exception.Message);

            var statusCode = exception switch
            {
                ArgumentException or ValidationException => StatusCodes.Status400BadRequest,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                InvalidOperationException => StatusCodes.Status422UnprocessableEntity,
                NotImplementedException => StatusCodes.Status501NotImplemented,
                _ => StatusCodes.Status500InternalServerError
            };

            var problemDetails = new ProblemDetails
            {
                Title = GetTitle(exception),
                Status = statusCode,
                Detail = exception.Message,
                Instance = httpContext.Request.Path,
                Type = exception.GetType().Name
            };

            if (httpContext.Request.Headers.ContainsKey("traceId"))
            {
                problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
            }

            httpContext.Response.ContentType = "application/problem+json";
            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
            return true;
        }

        private string GetTitle(Exception exception) => exception switch
        {
            ArgumentException => "Invalid argument provided",
            ValidationException => "Validation error",
            KeyNotFoundException => "Resource not found",
            UnauthorizedAccessException => "Authentication or authorization failed",
            InvalidOperationException => "Invalid operation",
            NotImplementedException => "Feature not implemented",
            _ => "An unexpected error occurred"
        };
    }
}