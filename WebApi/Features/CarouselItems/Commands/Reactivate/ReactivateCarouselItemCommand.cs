using MediatR;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Common.Results;

namespace WebApi.Features.CarouselItems.Commands.Reactivate
{
    public record ReactivateCarouselItemCommand(
        int Id
    ) : IRequest<Result<int>>;

    public class ReactivateCarouselItemCommandHandler : IRequestHandler<ReactivateCarouselItemCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;
        public ReactivateCarouselItemCommandHandler(IApplicationDbContext context, ILocalizationService localizationService)
        {
            _context = context;
            _localizationService = localizationService;
        }
        public async Task<Result<int>> Handle(ReactivateCarouselItemCommand request, CancellationToken cancellationToken)
        {
            var carouselItem = await _context.CarouselItems.FindAsync(request.Id);
            if (carouselItem == null)
                return Result<int>.Failure(_localizationService.GetLocalizedString(LocalizationKeys.CarouselItems.NotFound));

            carouselItem.IsActive = true;
            await _context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(carouselItem.Id, _localizationService.GetLocalizedString(LocalizationKeys.CarouselItems.Reactivated));
        }
    }
}
