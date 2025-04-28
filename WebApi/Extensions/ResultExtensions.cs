using WebApi.Extensions;
using WebApi.Common.Results;

namespace WebApi.Common.Extensions;

public static class ResultExtensions
{
    public static IResult ToApiResult<T>(this Result<T> result)
    {
        if (result.Succeeded)
            return Microsoft.AspNetCore.Http.Results.Ok(
                new { data = result.Data, message = result.Message });

        return CreateProblemResult(result.Error, result.ErrorType);
    }

    public static IResult ToApiResult(this Result result)
    {
        if (result.Succeeded)
            return result.Message != null
                ? Microsoft.AspNetCore.Http.Results.Ok(new { message = result.Message })
                : Microsoft.AspNetCore.Http.Results.Ok();

        return CreateProblemResult(result.Error, result.ErrorType);
    }

    private static IResult CreateProblemResult(string? detail, ResultErrorType errorType)
    {
        var statusCode = errorType switch
        {
            ResultErrorType.NotFound => StatusCodes.Status404NotFound,
            ResultErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ResultErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ResultErrorType.Conflict => StatusCodes.Status409Conflict,
            ResultErrorType.Validation => StatusCodes.Status400BadRequest,
            ResultErrorType.BadRequest => StatusCodes.Status400BadRequest,
            ResultErrorType.InternalServerError => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status400BadRequest
        };

        return Microsoft.AspNetCore.Http.Results.Problem(
            detail: detail ?? "An unexpected error occurred.",
            statusCode: statusCode
        );
    }

    // Extension method for validation failures
    public static Result<T> ToValidationResult<T>(this FluentValidation.Results.ValidationResult validationResult)
    {
        var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
        return Result<T>.ValidationError(errors);
    }
}
