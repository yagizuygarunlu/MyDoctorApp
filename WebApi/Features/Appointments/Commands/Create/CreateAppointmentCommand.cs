using FluentValidation;
using MediatR;
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
        public CreateAppointmentCommandValidator()
        {
            RuleFor(x => x.DoctorId)
                .NotEmpty()
                .WithMessage("Doctor ID is required.")
                .GreaterThan(0)
                .WithMessage("Doctor ID must be greater than 0.");
            RuleFor(x => x.PatientName)
                .NotEmpty()
                .WithMessage("Patient name is required.");
            RuleFor(x => x.PatientEmail)
                .NotEmpty()
                .WithMessage("Patient email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.");
            RuleFor(x => x.PatientPhone)
                .NotEmpty()
                .WithMessage("Patient phone number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Invalid phone number format.");
            RuleFor(x => x.AppointmentDate)
                .NotEmpty()
                .WithMessage("Appointment date is required.")
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Appointment date must be in the future.");
        }
    }
    public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, Result<Guid>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreateAppointmentCommandHandler(ApplicationDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }
        public async Task<Result<Guid>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var patientAddedResult = await _mediator.Send(new CreatePatientCommand(
                 request.PatientName,
                 request.PatientEmail,
                 request.PatientPhone
             ), cancellationToken);

            if (!patientAddedResult.Succeeded)
                return Result<Guid>.Failure("Failed to create patient.");

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
