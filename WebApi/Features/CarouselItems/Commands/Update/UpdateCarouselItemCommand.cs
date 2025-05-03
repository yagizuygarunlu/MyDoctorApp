using FluentValidation;
using MediatR;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Common.Results;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.CarouselItems.Commands.Update
{
    public record UpdateCarouselItemCommand(
        int Id,
        string Title,
        string Description,
        string ImageUrl,
        int DisplayOrder
        ) : IRequest<Result<int>>;

    public class UpdateCarouselItemCommandValidator : AbstractValidator<UpdateCarouselItemCommand>
    {
        public UpdateCarouselItemCommandValidator(ILocalizationService _localizationService)
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.CarouselItems.TitleRequired));
            RuleFor(x => x.ImageUrl)
                .NotEmpty()
                .WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.CarouselItems.ImageUrlRequired));

            RuleFor(x => x.DisplayOrder)
                .GreaterThan(0)
                .WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.CarouselItems.InvalidDisplayOrder));
        }
    }

    public class UpdateCarouselItemCommandHandler : IRequestHandler<UpdateCarouselItemCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;
        public UpdateCarouselItemCommandHandler(IApplicationDbContext context, ILocalizationService localizationService)
        {
            _context = context;
            _localizationService = localizationService;
        }
        public async Task<Result<int>> Handle(UpdateCarouselItemCommand request, CancellationToken cancellationToken)
        {
            var carouselItem = await _context.CarouselItems.FindAsync(request.Id);
            if (carouselItem == null)
                return Result<int>.Failure(_localizationService.GetLocalizedString(LocalizationKeys.CarouselItems.NotFound));

            carouselItem.Title = request.Title;
            carouselItem.Description = request.Description;
            carouselItem.ImageUrl = request.ImageUrl;
            carouselItem.DisplayOrder = request.DisplayOrder;
            await _context.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(carouselItem.Id, _localizationService.GetLocalizedString(LocalizationKeys.CarouselItems.Updated));
        }
    }
}
