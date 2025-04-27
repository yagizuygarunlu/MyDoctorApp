using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using WebApi.Common.Localization;

namespace WebApi.Exceptions
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly ILocalizationService _localizationService;
        private static readonly ActivitySource _activitySource = new("WebApi.Exceptions");
        private static readonly TextMapPropagator _propagator = Propagators.DefaultTextMapPropagator;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, ILocalizationService localizationService)
        {
            _logger = logger;
            _localizationService = localizationService;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var traceId = Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier;

            using var activity = _activitySource.StartActivity("HandleException", ActivityKind.Internal);
            activity?.SetTag("exception.type", exception.GetType().Name);
            activity?.SetTag("exception.message", exception.Message);
            activity?.SetStatus(ActivityStatusCode.Error, exception.Message);

            Log.ForContext("TraceId", traceId)
               .ForContext("ExceptionType", exception.GetType().Name)
               .ForContext("Path", httpContext.Request.Path)
               .ForContext("Method", httpContext.Request.Method)
               .ForContext("UserId", httpContext.User.Identity?.Name ?? "anonymous")
               .Error(exception, "Exception occurred during request processing: {ExceptionMessage}", exception.Message);

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
                Title = GetLocalizedTitle(exception),
                Status = statusCode,
                Detail = exception.Message,
                Instance = httpContext.Request.Path,
                Type = exception.GetType().Name
            };

            problemDetails.Extensions["traceId"] = traceId;

            if (httpContext.RequestServices.GetService<IWebHostEnvironment>()?.IsDevelopment() == true)
            {
                problemDetails.Extensions["stackTrace"] = exception.StackTrace;
            }

            httpContext.Response.ContentType = "application/problem+json";
            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
            return true;
        }

        private string GetLocalizedTitle(Exception exception) => exception switch
        {
            ArgumentException => _localizationService.GetLocalizedString(LocalizationKeys.Exceptions.InvalidArgument),
            ValidationException => _localizationService.GetLocalizedString(LocalizationKeys.Exceptions.ValidationError),
            KeyNotFoundException => _localizationService.GetLocalizedString(LocalizationKeys.Exceptions.ResourceNotFound),
            UnauthorizedAccessException => _localizationService.GetLocalizedString(LocalizationKeys.Exceptions.AuthorizationFailed),
            InvalidOperationException => _localizationService.GetLocalizedString(LocalizationKeys.Exceptions.InvalidOperation),
            NotImplementedException => _localizationService.GetLocalizedString(LocalizationKeys.Exceptions.NotImplemented),
            _ => _localizationService.GetLocalizedString(LocalizationKeys.Exceptions.UnexpectedError)
        };
    }
}
