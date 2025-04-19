using MediatR;
using WebApi.Common.Localization;
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
        private readonly ILocalizationService _localizationService;

        public DeleteTreatmentFaqCommandHandler(ApplicationDbContext context, ILocalizationService localizationService)
        {
            _context = context;
            _localizationService = localizationService;
        }

        public async Task<Result<Unit>> Handle(DeleteTreatmentFaqCommand request, CancellationToken cancellationToken)
        {
            var treatmentFaq = await _context.TreatmentFaqs.FindAsync(request.Id);
            if (treatmentFaq == null)
            {
                return Result<Unit>.Failure(_localizationService.GetLocalizedString(LocalizationKeys.TreatmentFaqs.NotFound));
            }
            _context.TreatmentFaqs.Remove(treatmentFaq);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<Unit>.Success(Unit.Value,_localizationService.GetLocalizedString(LocalizationKeys.TreatmentFaqs.Deleted));
        }
    }
}
