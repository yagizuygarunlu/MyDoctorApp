using MediatR;
using WebApi.Common.Results;
using WebApi.Infrastructure.Persistence;
using WebApi.Common.Localization;

namespace WebApi.Features.Treatments.Commands.Reactivate
{
    public record ReactivateTreatmentCommand(
        int Id
    ) : IRequest<Result<int>>;

    public class ReactivateTreatmentCommandHandler : IRequestHandler<ReactivateTreatmentCommand, Result<int>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;

        public ReactivateTreatmentCommandHandler(ApplicationDbContext context, ILocalizationService localizationService)
        {
            _context = context;
            _localizationService = localizationService;
        }

        public async Task<Result<int>> Handle(ReactivateTreatmentCommand request, CancellationToken cancellationToken)
        {
            var treatment = await _context.Treatments.FindAsync(request.Id);
            if (treatment == null)
            {
                return Result<int>.Failure(_localizationService.GetLocalizedString(LocalizationKeys.Treatments.NotFound));
            }

            treatment.IsActive = true;
            await _context.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(treatment.Id, _localizationService.GetLocalizedString(LocalizationKeys.Treatments.Reactivated));
        }
    }
}
