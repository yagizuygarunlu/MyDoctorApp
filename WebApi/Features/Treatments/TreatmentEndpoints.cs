using MediatR;
using WebApi.Common.Extensions;
using WebApi.Common.Localization;
using WebApi.Features.Treatments.Commands.Create;
using WebApi.Features.Treatments.Commands.Delete;
using WebApi.Features.Treatments.Commands.Reactivate;
using WebApi.Features.Treatments.Commands.Update;
using WebApi.Features.Treatments.Queries.GetActives;
using WebApi.Features.Treatments.Queries.GetAll;

namespace WebApi.Features.Treatments
{
    public static class TreatmentEndpoints
    {
        public static void MapTreatmentEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/treatments").WithTags("Treatments");

            group.MapGet("/", async (IMediator mediator) =>
            {
                var result = await mediator.Send(new GetAllTreatmentsQuery());
                return result.ToApiResult();
            });
            group.MapGet("/active", async (IMediator mediator) =>
            {
                var result = await mediator.Send(new GetActiveTreatmentsQuery());
                return result.ToApiResult();
            });
            group.MapPost("/", async (CreateTreatmentCommand command, IMediator mediator) =>
            {
                var result = await mediator.Send(command);
                return result.ToApiResult();
            });
            group.MapPut("/{id:int}", async (int id, UpdateTreatmentCommand command, IMediator mediator, ILocalizationService localizationService) =>
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
                var result = await mediator.Send(new DeleteTreatmentCommand(id));
                return result.ToApiResult();
            });
            group.MapPut("/{id:int}/reactivate", async (int id, IMediator mediator) =>
            {
                var result = await mediator.Send(new ReactivateTreatmentCommand(id));
                return result.ToApiResult();
            });
        }
    }
}
