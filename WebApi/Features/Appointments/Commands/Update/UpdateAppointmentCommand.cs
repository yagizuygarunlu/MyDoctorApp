using FluentValidation;
using MediatR;
using WebApi.Common.Results;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.Appointments.Commands.Update
{
    public record UpdateAppointmentCommand(
        int Id,
        int DoctorId,
        string PatientName,
        string PatientEmail,
        string PatientPhone,
        DateTime AppointmentDate,
        string? Notes = null
    ) : IRequest<Result<Unit>>;

    public class UpdateAppointmentCommandValidator : AbstractValidator<UpdateAppointmentCommand>
    {
        public UpdateAppointmentCommandValidator()
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

    public class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand, Result<Unit>>
    {
        private readonly ApplicationDbContext _context;
        public UpdateAppointmentCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<Unit>> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _context.Appointments.FindAsync(request.Id);
            if (appointment == null)
                return Result<Unit>.Failure("Appointment not found.");

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
