using MediatR;
using WebApi.Common.Localization;
using WebApi.Common.Results;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.Appointments.Commands.Reject
{
    public record RejectAppointmentCommand(
        int Id,
        string Reason
    ) : IRequest<Result<Unit>>;

    public sealed class RejectAppointmentCommandHandler : IRequestHandler<RejectAppointmentCommand, Result<Unit>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;
        public RejectAppointmentCommandHandler(
            ApplicationDbContext context,
            ILocalizationService localizationService)
        {
            _context = context;
            _localizationService = localizationService;
        }
        public async Task<Result<Unit>> Handle(RejectAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _context.Appointments.FindAsync(request.Id);
            if (appointment == null)
            {
                return Result<Unit>.Failure(_localizationService.GetLocalizedString(_localizationService.GetLocalizedString(LocalizationKeys.Appointments.Rejected)));
            }
            appointment.Status = Domain.Enums.AppointmentStatus.Rejected;
            appointment.RejectionReason = request.Reason;
            await _context.SaveChangesAsync(cancellationToken);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
