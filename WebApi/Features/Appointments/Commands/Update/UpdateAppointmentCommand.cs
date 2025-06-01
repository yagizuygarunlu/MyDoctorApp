using FluentValidation;
using MediatR;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Common.Results;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.Appointments.Commands.Update
{
    public record UpdateAppointmentCommand(
        Guid Id,
        int DoctorId,
        string PatientName,
        string PatientEmail,
        string PatientPhone,
        DateTime AppointmentDate,
        string? Notes = null
    ) : IRequest<Result<Unit>>;

    public class UpdateAppointmentCommandValidator : AbstractValidator<UpdateAppointmentCommand>
    {
        public UpdateAppointmentCommandValidator(ILocalizationService _localizationService)
        {
            RuleFor(x => x.DoctorId)
                .NotEmpty()
                .WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Appointments.DoctorRequired))
                .GreaterThan(0)
                .WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Appointments.DoctorIdMustBeGreaterThanZero));
            RuleFor(x => x.PatientName)
                .NotEmpty()
                .WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Patients.NameRequired));
            RuleFor(x => x.PatientEmail)
                .NotEmpty()
                .WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Patients.EmailRequired))
                .EmailAddress()
                .WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Patients.InvalidEmail))
                .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
                .WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Patients.InvalidEmail));
            RuleFor(x => x.PatientPhone)
                .NotEmpty()
                .WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Patients.PhoneRequired))
                .Matches(@"^\+?[1-9]\d{9,14}$")
                .WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Patients.InvalidPhone));
            RuleFor(x => x.AppointmentDate)
                .NotEmpty()
                .WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Appointments.DateRequired))
                .GreaterThan(DateTime.UtcNow)
                .WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Appointments.DateMustBeInFuture));
        }
    }

    public class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand, Result<Unit>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;
        public UpdateAppointmentCommandHandler(
            IApplicationDbContext context,
            ILocalizationService localizationService)
        {
            _context = context;
            _localizationService = localizationService;
        }
        public async Task<Result<Unit>> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _context.Appointments.FindAsync(request.Id);
            if (appointment == null)
                return Result<Unit>.Failure(_localizationService.GetLocalizedString(LocalizationKeys.Appointments.NotFound));

            var patient = await _context.Patients.FindAsync(appointment.PatientId);
            patient.FullName = request.PatientName;
            patient.PhoneNumber = request.PatientPhone;
            patient.Email = request.PatientEmail;
            appointment.DoctorId = request.DoctorId;
            appointment.Date = request.AppointmentDate;
            appointment.Description = request.Notes;
            await _context.SaveChangesAsync(cancellationToken);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
