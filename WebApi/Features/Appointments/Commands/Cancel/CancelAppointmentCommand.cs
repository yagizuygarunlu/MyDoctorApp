using MediatR;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Common.Results;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.Appointments.Commands.Cancel
{
    public record CancelAppointmentCommand(
        int Id
    ) : IRequest<Result<Unit>>;

    public sealed class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, Result<Unit>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;
        public CancelAppointmentCommandHandler(
            IApplicationDbContext context,
            ILocalizationService localizationService)
        {
            _context = context;
            _localizationService = localizationService;
        }
        public async Task<Result<Unit>> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _context.Appointments.FindAsync(request.Id);
            if (appointment == null)
            {
                return Result<Unit>.Failure(_localizationService.GetLocalizedString(LocalizationKeys.Appointments.Cancelled));
            }
            appointment.Status = Domain.Enums.AppointmentStatus.Cancelled;
            await _context.SaveChangesAsync(cancellationToken);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
