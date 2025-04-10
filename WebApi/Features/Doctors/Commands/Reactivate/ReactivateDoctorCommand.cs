using MediatR;
using WebApi.Common.Results;
using WebApi.Infrastructure.Persistence;
using FluentValidation;

namespace WebApi.Features.Doctors.Commands.Reactivate
{
    public record ReactivateDoctorCommand(
        int Id
    ) : IRequest<Result<int>>;

    public class ReactivateDoctorValidator : AbstractValidator<ReactivateDoctorCommand>
    {
        public ReactivateDoctorValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than zero.");
        }
    }

    public sealed class ReactivateDoctorHandler : IRequestHandler<ReactivateDoctorCommand, Result<int>>
    {
        private readonly ApplicationDbContext _dbContext;
        public ReactivateDoctorHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Result<int>> Handle(ReactivateDoctorCommand request, CancellationToken cancellationToken)
        {
            var doctor = await _dbContext.Doctors.FindAsync(request.Id);
            if (doctor == null)
            {
                return Result<int>.Failure("Doctor not found.");
            }
            doctor.IsActive = true;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(request.Id);
        }
    }
}
