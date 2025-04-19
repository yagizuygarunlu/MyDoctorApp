using Microsoft.AspNetCore.Mvc;
using WebApi.Common.Results;

namespace WebApi.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToHttpResult<T>(this Result<T> result)
        {
            if (!result.Succeeded)
                return new BadRequestObjectResult(result.Errors);

            return new OkObjectResult(result.Data);
        }
    }
}
