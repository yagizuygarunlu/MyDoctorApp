using WebApi.Extensions;

namespace WebApi.Common.Results
{
    public class Result
    {
        public bool Succeeded { get; set; }
        public string? Error { get; set; }
        public ResultErrorType ErrorType { get; set; } = ResultErrorType.Unknown;

        public static Result Success() => new Result { Succeeded = true };

        public static Result Failure(string error, ResultErrorType errorType = ResultErrorType.Unknown)
            => new Result { Succeeded = false, Error = error, ErrorType = errorType };
    }


    public class Result<T>
    {
        public bool Succeeded { get; set; }
        public T? Data { get; set; }
        public string? Error { get; set; }
        public ResultErrorType ErrorType { get; set; } = ResultErrorType.Unknown;

        public static Result<T> Success(T data) => new Result<T> { Succeeded = true, Data = data };

        public static Result<T> Failure(string error, ResultErrorType errorType = ResultErrorType.Unknown)
            => new Result<T> { Succeeded = false, Error = error, ErrorType = errorType };
    }

}
