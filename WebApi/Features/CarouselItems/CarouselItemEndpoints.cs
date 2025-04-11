using MediatR;
using WebApi.Features.CarouselItems.Commands.Create;
using WebApi.Features.CarouselItems.Commands.Delete;
using WebApi.Features.CarouselItems.Commands.Reactivate;
using WebApi.Features.CarouselItems.Commands.Update;
using WebApi.Features.CarouselItems.Queries.GetActives;
using WebApi.Features.CarouselItems.Queries.GetAll;

namespace WebApi.Features.CarouselItems
{
    public static class CarouselItemEndpoints
    {
        public static void MapCarouselItemEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/carousel-items").WithTags("CarouselItems");
            group.MapGet("/", async (IMediator mediator) =>
            {
                var result = await mediator.Send(new GetAllCarouselItemsQuery());
                return Results.Ok(result);
            });
            group.MapGet("/active", async (IMediator mediator) =>
            {
                var result = await mediator.Send(new GetActiveCarouselItemsQuery());
                return Results.Ok(result);
            });
            group.MapPost("/", async (CreateCarouselItemCommand command, IMediator mediator) =>
            {
                var result = await mediator.Send(command);
                return Results.Created($"/api/carousel-items/{result.Data}", result.Data);
            });
            group.MapPut("/{id:int}", async (int id, UpdateCarouselItemCommand command, IMediator mediator) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("Id in the URL does not match the Id in the body.");
                }
                var result = await mediator.Send(command);
                return Results.Ok(result);
            });
            group.MapDelete("/{id:int}", async (int id, IMediator mediator) =>
            {
                await mediator.Send(new DeleteCarouselItemCommand(id));
                return Results.NoContent();
            });
            group.MapPut("/{id:int}/reactivate", async (int id, IMediator mediator) =>
            {
                var result = await mediator.Send(new ReactivateCarouselItemCommand(id));
                return Results.Ok(result);
            });
        }
    }
}
