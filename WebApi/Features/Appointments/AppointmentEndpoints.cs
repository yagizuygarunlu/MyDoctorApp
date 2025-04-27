using MediatR;
using Microsoft.AspNetCore.Mvc;
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
            
            // Use [AsParameters] to bind query parameters to GetAppointmentsQuery properties
            group.MapGet("/", async ([FromServices] IMediator mediator, [AsParameters] GetAppointmentsQuery query, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(query, cancellationToken);
                return Results.Ok(result);
            });
            
            group.MapGet("/todays", async ([FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new GetTodaysAppointmentsQuery(), cancellationToken);
                return Results.Ok(result);
            });
            
            group.MapPost("/", async ([FromServices] IMediator mediator, [FromBody] CreateAppointmentCommand command, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(command, cancellationToken);
                return Results.Ok(result);
            });
            
            group.MapPut("/{id:int}/approve", async ([FromServices] IMediator mediator, int id, string doctorId, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new ApproveAppointmentCommand(id, doctorId), cancellationToken);
                return Results.Ok(result);
            });
            
            group.MapPut("/{id:int}/cancel", async ([FromServices] IMediator mediator, int id, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new CancelAppointmentCommand(id), cancellationToken);
                return Results.Ok(result);
            });
            
            group.MapPut("/{id:int}/reject", async ([FromServices] IMediator mediator, [FromBody] RejectAppointmentCommand command, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(command, cancellationToken);
                return Results.Ok(result);
            });
        }
    }
}
