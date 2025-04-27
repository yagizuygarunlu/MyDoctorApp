using MediatR;
using WebApi.Common.Extensions;
using WebApi.Features.Reviews.Commands;

namespace WebApi.Features.Reviews
{
    public static class ReviewEndpoints
    {
        public static void MapReviewEndpoints(this IEndpointRouteBuilder routes)
        {
            routes.MapPost("/reviews", async (CreateReviewCommand command, IMediator mediator) =>
            {
                var result = await mediator.Send(command);
                return result.ToApiResult();
            })
            .WithName("CreateReview")
            .Produces<int>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithTags("Reviews");
        }
    }
}
