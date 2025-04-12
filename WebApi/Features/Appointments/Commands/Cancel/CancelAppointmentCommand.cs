using MediatR;
using WebApi.Common.Results;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.Appointments.Commands.Cancel
{
    public record CancelAppointmentCommand(
        int Id
    ) : IRequest<Result<Unit>>;

    public sealed class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, Result<Unit>>
    {
        private readonly ApplicationDbContext _context;
        public CancelAppointmentCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<Unit>> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _context.Appointments.FindAsync(request.Id);
            if (appointment == null)
            {
                return Result<Unit>.Failure("Appointment not found.");
            }
            appointment.IsCancelled = true;
            await _context.SaveChangesAsync(cancellationToken);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
