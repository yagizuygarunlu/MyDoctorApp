using MediatR;
using WebApi.Common.Localization;
using WebApi.Common.Results;
using WebApi.Domain.Enums;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.Appointments.Commands.Approve
{
    public record ApproveAppointmentCommand(
        int AppointmentId,
        string DoctorId) : IRequest<Result<Unit>>;

    public sealed class ApproveAppointmentCommandHandler : IRequestHandler<ApproveAppointmentCommand, Result<Unit>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;
        public ApproveAppointmentCommandHandler(
            ApplicationDbContext context,
            ILocalizationService localizationService)
        {
            _context = context;
            _localizationService = localizationService;
        }
        public async Task<Result<Unit>> Handle(ApproveAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _context.Appointments
                .FindAsync(new object[] { request.AppointmentId }, cancellationToken);
            if (appointment == null)
            {
                return Result<Unit>.Failure(_localizationService.GetLocalizedString(LocalizationKeys.Appointments.NotFound));
            }
            appointment.Status = AppointmentStatus.Approved;
            await _context.SaveChangesAsync(cancellationToken);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
