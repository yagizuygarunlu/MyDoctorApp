using MediatR;
using WebApi.Common.Extensions;
using WebApi.Common.Localization;
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
                return result.ToApiResult();
            });
            group.MapGet("/active", async (IMediator mediator) =>
            {
                var result = await mediator.Send(new GetActiveCarouselItemsQuery());
                return result.ToApiResult();
            });
            group.MapPost("/", async (CreateCarouselItemCommand command, IMediator mediator) =>
            {
                var result = await mediator.Send(command);
                return result.ToApiResult();
            });
            group.MapPut("/{id:int}", async (int id, UpdateCarouselItemCommand command, IMediator mediator, ILocalizationService localizationService) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest(localizationService.GetLocalizedString(LocalizationKeys.Common.IdMismatch));
                }
                var result = await mediator.Send(command);
                return result.ToApiResult();
            });
            group.MapDelete("/{id:int}", async (int id, IMediator mediator) =>
            {
                var result = await mediator.Send(new DeleteCarouselItemCommand(id));
                return result.ToApiResult();
            });
            group.MapPut("/{id:int}/reactivate", async (int id, IMediator mediator) =>
            {
                var result = await mediator.Send(new ReactivateCarouselItemCommand(id));
                return result.ToApiResult();
            });
        }
    }
}
