using MediatR;
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
                return result.Succeeded ? Results.Created($"/reviews/{result.Data}", result.Data) : Results.BadRequest(result.Errors);
            })
            .WithName("CreateReview")
            .Produces<int>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithTags("Reviews");
        }
    }
}
