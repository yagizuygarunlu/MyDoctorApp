using MediatR;
using WebApi.Common.Localization;
using WebApi.Common.Results;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.CarouselItems.Commands.Delete
{
    public record DeleteCarouselItemCommand(
        int Id
    ) : IRequest<Result<Unit>>;

    public class DeleteCarouselItemCommandHandler : IRequestHandler<DeleteCarouselItemCommand, Result<Unit>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;
        public DeleteCarouselItemCommandHandler(ApplicationDbContext context, ILocalizationService localizationService)
        {
            _context = context;
            _localizationService = localizationService;
        }
        public async Task<Result<Unit>> Handle(DeleteCarouselItemCommand request, CancellationToken cancellationToken)
        {
            var carouselItem = await _context.CarouselItems.FindAsync(request.Id);
            if (carouselItem == null)
                return Result<Unit>.Failure(_localizationService.GetLocalizedString(LocalizationKeys.CarouselItems.NotFound));

            carouselItem.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);
            return Result<Unit>.Success(Unit.Value,_localizationService.GetLocalizedString(LocalizationKeys.CarouselItems.Deleted)); // Explicitly pass Unit.Value to match the expected type
        }
    }
}
