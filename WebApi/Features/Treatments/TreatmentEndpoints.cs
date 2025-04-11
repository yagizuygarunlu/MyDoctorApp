using MediatR;
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
                return Results.Ok(result);
            });
            group.MapGet("/active", async (IMediator mediator) =>
            {
                var result = await mediator.Send(new GetActiveTreatmentsQuery());
                return Results.Ok(result);
            });
            group.MapPost("/", async (CreateTreatmentCommand command, IMediator mediator) =>
            {
                var result = await mediator.Send(command);
                return Results.Created($"/api/treatments/{result.Data}", result.Data);
            });
            group.MapPut("/{id:int}", async (int id, UpdateTreatmentCommand command, IMediator mediator) =>
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
                await mediator.Send(new DeleteTreatmentCommand(id));
                return Results.NoContent();
            });
            group.MapPut("/{id:int}/reactivate", async (int id, IMediator mediator) =>
            {
                var result = await mediator.Send(new ReactivateTreatmentCommand(id));
                return Results.Ok(result);
            });
        }
    }
}
