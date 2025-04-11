using MediatR;
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
                return Results.Created($"/api/doctors/{result.Data}", result.Data);
            });

            group.MapPut("/{id:int}", async (int id, UpdateDoctorCommand command, IMediator mediator) =>
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
                var result = await mediator.Send(new DeleteDoctorCommand(id));
                return result;
            });

            group.MapPut("/{id:int}/reactivate", async (int id, IMediator mediator) =>
            {
                var result = await mediator.Send(new ReactivateDoctorCommand(id));
                return result;
            });
        }
    }
}
