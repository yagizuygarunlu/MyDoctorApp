using MediatR;
using WebApi.Common.Results;
using WebApi.Infrastructure.Persistence;
using WebApi.Common.Localization;
using WebApi.Application.Common.Interfaces;

namespace WebApi.Features.Treatments.Commands.Delete
{
    public record DeleteTreatmentCommand(
        int Id
    ) : IRequest<Result<Unit>>;

    public class DeleteTreatmentCommandHandler : IRequestHandler<DeleteTreatmentCommand, Result<Unit>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;

        public DeleteTreatmentCommandHandler(IApplicationDbContext context, ILocalizationService localizationService)
        {
            _context = context;
            _localizationService = localizationService;
        }

        public async Task<Result<Unit>> Handle(DeleteTreatmentCommand request, CancellationToken cancellationToken)
        {
            var treatment = await _context.Treatments.FindAsync(request.Id);
            if (treatment == null)
            {
                return Result<Unit>.Failure(_localizationService.GetLocalizedString(LocalizationKeys.Treatments.NotFound));
            }

            treatment.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value, _localizationService.GetLocalizedString(LocalizationKeys.Treatments.Deleted));
        }
    }
}
