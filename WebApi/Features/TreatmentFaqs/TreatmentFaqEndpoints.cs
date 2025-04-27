using MediatR;
using WebApi.Common.Localization;

namespace WebApi.Features.TreatmentFaqs
{
    public static class TreatmentFaqEndpoints
    {
        public static void MapTreatmentFaqEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/treatment-faqs").WithTags("Treatment FAQs");
            group.MapGet("/", async (IMediator mediator) =>
            {
                var result = await mediator.Send(new Queries.GetAll.GetAllTreatmentFaqsQuery());
                return Results.Ok(result);
            });
            group.MapPost("/", async (IMediator mediator, Commands.Create.CreateTreatmentFaqCommand command) =>
            {
                var result = await mediator.Send(command);
                return Results.Ok(result);
            });
            group.MapPut("/{id}", async (IMediator mediator, int id, Commands.Update.UpdateTreatmentFaqCommand command, ILocalizationService localizationService) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest(localizationService.GetLocalizedString(LocalizationKeys.Common.IdMismatch));
                }
                var result = await mediator.Send(command);
                return Results.Ok(result);
            });
            group.MapDelete("/{id}", async (IMediator mediator, int id) =>
            {
                var result = await mediator.Send(new Commands.Delete.DeleteTreatmentFaqCommand(id));
                return Results.Ok(result);
            });
        }
    }
}
