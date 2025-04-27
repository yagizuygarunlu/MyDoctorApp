using WebApi.Common.Results;
using WebApi.Extensions;

namespace WebApi.Common.Extensions;

public static class ResultExtensions
{
    public static IResult ToApiResult<T>(this Result<T> result)
    {
        if (result.Succeeded)
            return Microsoft.AspNetCore.Http.Results.Ok(result.Data);

        return CreateProblemResult(result.Error, result.ErrorType);
    }

    public static IResult ToApiResult(this Result result)
    {
        if (result.Succeeded)
            return Microsoft.AspNetCore.Http.Results.Ok();

        return CreateProblemResult(result.Error, result.ErrorType);
    }

    private static IResult CreateProblemResult(string? detail, ResultErrorType errorType)
    {
        var statusCode = errorType switch
        {
            ResultErrorType.NotFound => 404,
            ResultErrorType.Forbidden => 403,
            ResultErrorType.Conflict => 409,
            ResultErrorType.Validation => 400,
            _ => 400
        };

        return Microsoft.AspNetCore.Http.Results.Problem(
            detail: detail ?? "An unexpected error occurred.",
            statusCode: statusCode
        );
    }
}
