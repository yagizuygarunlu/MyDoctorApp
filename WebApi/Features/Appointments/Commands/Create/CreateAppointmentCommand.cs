using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Domain.Enums;
using WebApi.Features.Patients.Commands.Create;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.Appointments.Commands.Create
{
    public record CreateAppointmentCommand(
        int DoctorId,
        string PatientName,
        string PatientEmail,
        string PatientPhone,
        DateTime AppointmentDate,
        string? Notes = null
    ) : IRequest<Result<Guid>>;

    public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;
        public CreateAppointmentCommandValidator(
           IApplicationDbContext context,
           ILocalizationService localizationService)
        {
            _context = context;
            _localizationService = localizationService;

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
                .WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Patients.InvalidEmail));
            RuleFor(x => x.PatientPhone)
                .NotEmpty()
                .WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Patients.PhoneRequired))
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Patients.InvalidPhone));
            RuleFor(x => x.AppointmentDate)
                .NotEmpty()
                .WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Appointments.DateRequired))
                .GreaterThan(DateTime.UtcNow)
                .WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Appointments.DateMustBeInFuture));

            RuleFor(x => x)
              .MustAsync(IsDoctorAvailableAsync)
              .WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Appointments.DoctorUnavailable))
              .MustAsync(IsPatientAvailableAsync)
                .WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Appointments.PatientAlreadyHasAppointment));
        }
        private async Task<bool> IsDoctorAvailableAsync(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointmentDateTimeOffset = new DateTimeOffset(request.AppointmentDate);
            var appointmentStart = appointmentDateTimeOffset.AddMinutes(-30);
            var appointmentEnd = appointmentDateTimeOffset.AddMinutes(30);

            return !await _context.Appointments
                .Where(a =>
                    a.DoctorId == request.DoctorId &&
                    a.Date >= appointmentStart &&
                    a.Date <= appointmentEnd &&
                    (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Approved))
                .AnyAsync(cancellationToken);
        }

        private async Task<bool> IsPatientAvailableAsync(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointmentDateTimeOffset = new DateTimeOffset(request.AppointmentDate);

            var existingPatient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Email == request.PatientEmail, cancellationToken);

            if (existingPatient == null)
                return true;
            
            return !await _context.Appointments
                .Where(a =>
                    a.PatientId == existingPatient.Id &&
                    a.Date.Date == appointmentDateTimeOffset.Date &&
                    (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Approved))
                .AnyAsync(cancellationToken);
        }
    }
    public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, Result<Guid>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;
        private readonly ILocalizationService _localizationService;

        public CreateAppointmentCommandHandler(
            IApplicationDbContext context,
            IMediator mediator,
            ILocalizationService localizationService)
        {
            _context = context;
            _mediator = mediator;
            _localizationService = localizationService;
        }
        public async Task<Result<Guid>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var patientAddedResult = await _mediator.Send(new CreatePatientCommand(
                 request.PatientName,
                 request.PatientEmail,
                 request.PatientPhone
             ), cancellationToken);

            if (!patientAddedResult.Succeeded)
                return Result<Guid>.Failure(_localizationService.GetLocalizedString(LocalizationKeys.Patients.CreatingFailed));

            var appointment = new Appointment
            {
                Date = request.AppointmentDate,
                Description = request.Notes,
                PatientId = patientAddedResult.Data,
                DoctorId = request.DoctorId
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(appointment.Id);
        }
    }
}
