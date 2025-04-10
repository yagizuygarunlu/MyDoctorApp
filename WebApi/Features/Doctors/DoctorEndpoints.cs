using MediatR;
using WebApi.Features.Doctors.Commands.Create;
using WebApi.Features.Doctors.Commands.Delete;
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
            })
            .WithName("CreateDoctor")
            .Produces<int>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

            group.MapPut("/{id:int}", async (int id, UpdateDoctorCommand command, IMediator mediator) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("Id in the URL does not match the Id in the body.");
                }
                var result = await mediator.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateDoctor")
            .Produces<int>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

            group.MapDelete("/{id:int}", async (int id, IMediator mediator) =>
            {
                var result = await mediator.Send(new DeleteDoctorCommand(id));
                return result;
            })
            .WithName("DeleteDoctor")
            .Produces<int>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        }
    }
}
