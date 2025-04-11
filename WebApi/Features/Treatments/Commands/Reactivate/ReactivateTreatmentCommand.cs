using MediatR;
using WebApi.Common.Results;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.Treatments.Commands.Reactivate
{
    public record ReactivateTreatmentCommand(
        int Id
    ) : IRequest<Result<int>>;

    public class ReactivateTreatmentCommandHandler : IRequestHandler<ReactivateTreatmentCommand, Result<int>>
    {
        private readonly ApplicationDbContext _context;
        public ReactivateTreatmentCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<int>> Handle(ReactivateTreatmentCommand request, CancellationToken cancellationToken)
        {
            var treatment = await _context.Treatments.FindAsync(request.Id);
            if (treatment == null)
            {
                return Result<int>.Failure("Treatment not found.");
            }
            treatment.IsActive = true;
            await _context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(treatment.Id);
        }
    }
}
