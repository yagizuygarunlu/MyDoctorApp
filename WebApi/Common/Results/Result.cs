using WebApi.Extensions;

namespace WebApi.Common.Results;

public class Result
{
    public bool Succeeded { get; private set; }
    public string? Error { get; private set; }
    public string? Message { get; private set; }
    public ResultErrorType ErrorType { get; private set; }

    // Helper properties
    public bool IsFailure => !Succeeded;
    public bool IsSuccess => Succeeded;

    private Result() { } // Prevent direct instantiation

    public static Result Success(string? message = null)
    {
        return new Result { Succeeded = true, Message = message };
    }

    public static Result Failure(string error, ResultErrorType errorType = ResultErrorType.Unknown)
    {
        return new Result { Succeeded = false, Error = error, ErrorType = errorType };
    }

    // Helper methods for specific error types
    public static Result NotFound(string error) => Failure(error, ResultErrorType.NotFound);
    public static Result ValidationError(string error) => Failure(error, ResultErrorType.Validation);
    public static Result Unauthorized(string error) => Failure(error, ResultErrorType.Unauthorized);
    public static Result Forbidden(string error) => Failure(error, ResultErrorType.Forbidden);
    public static Result Conflict(string error) => Failure(error, ResultErrorType.Conflict);
    public static Result BadRequest(string error) => Failure(error, ResultErrorType.BadRequest);
}

public class Result<T>
{
    public bool Succeeded { get; private set; }
    public T? Data { get; private set; }
    public string? Error { get; private set; }
    public string? Message { get; private set; }
    public ResultErrorType ErrorType { get; private set; }

    // Helper properties
    public bool IsFailure => !Succeeded;
    public bool IsSuccess => Succeeded;

    private Result() { } // Prevent direct instantiation

    public static Result<T> Success(T data, string? message = null)
    {
        return new Result<T> { Succeeded = true, Data = data, Message = message };
    }

    public static Result<T> Failure(string error, ResultErrorType errorType = ResultErrorType.Unknown)
    {
        return new Result<T> { Succeeded = false, Error = error, ErrorType = errorType };
    }

    // Helper methods for specific error types
    public static Result<T> NotFound(string error) => Failure(error, ResultErrorType.NotFound);
    public static Result<T> ValidationError(string error) => Failure(error, ResultErrorType.Validation);
    public static Result<T> Unauthorized(string error) => Failure(error, ResultErrorType.Unauthorized);
    public static Result<T> Forbidden(string error) => Failure(error, ResultErrorType.Forbidden);
    public static Result<T> Conflict(string error) => Failure(error, ResultErrorType.Conflict);
    public static Result<T> BadRequest(string error) => Failure(error, ResultErrorType.BadRequest);

    // Implicit conversion to allow returning T directly from Result<T>
    public static implicit operator T?(Result<T> result) => result.Data;
}
