using MediatR;
using WebApi.Common.Extensions;
using WebApi.Common.Localization;
using WebApi.Features.Doctors.Commands.Create;
using WebApi.Features.Doctors.Commands.Delete;
using WebApi.Features.Doctors.Commands.Reactivate;
using WebApi.Features.Doctors.Commands.Update;

namespace WebApi.Features.Doctors
{
    public static class DoctorEndpoints
    {
        public static void MapDoctorEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/doctors").WithTags("Doctors");

            group.MapPost("/", async (CreateDoctorCommand command, IMediator mediator) =>
            {
                var result = await mediator.Send(command);
                return result.ToApiResult();
            });

            group.MapPut("/{id:int}", async (int id, UpdateDoctorCommand command, IMediator mediator, ILocalizationService localizationService) =>
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
                var result = await mediator.Send(new DeleteDoctorCommand(id));
                return result.ToApiResult();
            });

            group.MapPut("/{id:int}/reactivate", async (int id, IMediator mediator) =>
            {
                var result = await mediator.Send(new ReactivateDoctorCommand(id));
                return result.ToApiResult();
            });
        }
    }
}

