using MediatR;
using WebApi.Features.Appointments.Commands.Approve;
using WebApi.Features.Appointments.Commands.Cancel;
using WebApi.Features.Appointments.Commands.Create;
using WebApi.Features.Appointments.Commands.Reject;
using WebApi.Features.Appointments.Queries.GetAppointments;
using WebApi.Features.Appointments.Queries.GetTodays;

namespace WebApi.Features.Appointments
{
    public static class AppointmentEndpoints
    {
        public static void MapAppointmentEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/appointments").WithTags("Appointments");
            group.MapGet("/", async (IMediator mediator, GetAppointmentsQuery query, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(query, cancellationToken);
                return Results.Ok(result);
            });
            group.MapGet("/todays", async (IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new GetTodaysAppointmentsQuery(), cancellationToken);
                return Results.Ok(result);
            });
            group.MapPost("/", async (IMediator mediator, CreateAppointmentCommand command, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(command, cancellationToken);
                return Results.Ok(result);
            });
            group.MapPut("/{id:int}/approve", async (IMediator mediator, int id, string doctorId, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new ApproveAppointmentCommand(id, doctorId), cancellationToken);
                return Results.Ok(result);
            });
            group.MapPut("/{id:int}/cancel", async (IMediator mediator, int id, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new CancelAppointmentCommand(id), cancellationToken);
                return Results.Ok(result);
            });
            group.MapPut("/{id:int}/reject", async (IMediator mediator, ApproveAppointmentCommand command, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(command,cancellationToken);
                return Results.Ok(result);
            });
        }
    }
}
