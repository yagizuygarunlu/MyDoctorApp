using FluentValidation;
using MediatR;
using WebApi.Common.Results;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.Doctors.Commands.Delete
{
    public record DeleteDoctorCommand(
        int Id
    ) : IRequest<Result<int>>;

    public class DeleteDoctorValidator : AbstractValidator<DeleteDoctorCommand>
    {
        public DeleteDoctorValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than zero.");
        }
    }

    public sealed class DeleteDoctorHandler : IRequestHandler<DeleteDoctorCommand, Result<int>>
    {
        private readonly ApplicationDbContext _dbContext;
        public DeleteDoctorHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Result<int>> Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
        {
            var doctor = await _dbContext.Doctors.FindAsync(request.Id);
            if (doctor == null)
            {
                return Result<int>.Failure("Doctor not found.");
            }
            doctor.IsActive = false;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(request.Id);
        }
    }
}
