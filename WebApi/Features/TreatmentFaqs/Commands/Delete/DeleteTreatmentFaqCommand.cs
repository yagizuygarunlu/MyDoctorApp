using MediatR;
using WebApi.Common.Results;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.TreatmentFaqs.Commands.Delete
{
    public record DeleteTreatmentFaqCommand(
        int Id
    ) : IRequest<Result<Unit>>;
    public sealed class DeleteTreatmentFaqCommandHandler : IRequestHandler<DeleteTreatmentFaqCommand, Result<Unit>>
    {
        private readonly ApplicationDbContext _context;
        public DeleteTreatmentFaqCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<Unit>> Handle(DeleteTreatmentFaqCommand request, CancellationToken cancellationToken)
        {
            var treatmentFaq = await _context.TreatmentFaqs.FindAsync(request.Id);
            if (treatmentFaq == null)
            {
                return Result<Unit>.Failure("Treatment FAQ not found.");
            }
            _context.TreatmentFaqs.Remove(treatmentFaq);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
