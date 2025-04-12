using FluentValidation;
using MediatR;
using WebApi.Common.Results;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.Patients.Commands.Update
{
    public record UpdatePatientCommand(
        Guid Id,
        string Name,
        string Phone,
        string Email
        ): IRequest<Result<Unit>>;
    
    public class UpdatePatientCommandValidator: AbstractValidator<UpdatePatientCommand>
    {
        public UpdatePatientCommandValidator()
        {
            RuleFor(x => x.Name)
               .NotEmpty()
               .WithMessage("Name is required.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.");

            RuleFor(x => x.Phone)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Invalid phone number format.");
        }
    }

    public sealed class UpdatePatientCommandHandler: IRequestHandler<UpdatePatientCommand,Result<Unit>>
    {
        private readonly ApplicationDbContext _context;
        public UpdatePatientCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Unit>> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
        {
            var patient = await _context.Patients.FindAsync(request.Id);
            if (patient == null)
            {
                return Result<Unit>.Failure("Patient not found");
            }

            patient.FullName = request.Name;
            patient.PhoneNumber = request.Phone;
            patient.Email = request.Email;
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
