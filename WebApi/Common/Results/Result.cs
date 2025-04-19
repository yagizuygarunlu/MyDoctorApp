namespace WebApi.Common.Results
{
    public class Result
    {
        public bool Succeeded { get; set; }
        public string[] Errors { get; set; } = [];
        public string? SuccessMessage { get; set; }

        public static Result Success() => new() { Succeeded = true };
        public static Result Success(string message) => new() { Succeeded = true, SuccessMessage = message };
        public static Result Failure(params string[] errors) => new() { Succeeded = false, Errors = errors };
    }

    public class Result<T> : Result
    {
        public T Data { get; set; } = default!;

        public static Result<T> Success(T data) => new() { Succeeded = true, Data = data };
        public static Result<T> Success(T data, string message) => new() { Succeeded = true, Data = data, SuccessMessage = message };
        public static new Result<T> Failure(params string[] errors) => new() { Succeeded = false, Errors = errors };
    }
}
