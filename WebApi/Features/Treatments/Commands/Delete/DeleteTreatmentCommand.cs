using MediatR;
using WebApi.Common.Results;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.Treatments.Commands.Delete
{
    public record DeleteTreatmentCommand(
        int Id
    ) : IRequest<Result<Unit>>;

    public class DeleteTreatmentCommandHandler : IRequestHandler<DeleteTreatmentCommand, Result<Unit>>
    {
        private readonly ApplicationDbContext _context;
        public DeleteTreatmentCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<Unit>> Handle(DeleteTreatmentCommand request, CancellationToken cancellationToken)
        {
            var treatment = await _context.Treatments.FindAsync(request.Id);
            if (treatment == null)
            {
                return Result<Unit>.Failure("Treatment not found.");
            }
            treatment.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
