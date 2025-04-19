using FluentValidation;
using MediatR;
using WebApi.Common.Localization;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
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
        public CreateAppointmentCommandValidator(ILocalizationService _localizationService)
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
        }
    }
    public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, Result<Guid>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;
        private readonly ILocalizationService _localizationService;

        public CreateAppointmentCommandHandler(
            ApplicationDbContext context,
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
