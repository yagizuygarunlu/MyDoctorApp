using FluentValidation;
using MediatR;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.CarouselItems.Commands.Create
{
    public record CreateCarouselItemCommand(
        string Title,
        string Description,
        string ImageUrl,
        int DisplayOrder
    ) : IRequest<Result<int>>;

    public class CreateCarouselItemCommandValidator : AbstractValidator<CreateCarouselItemCommand>
    {
        public CreateCarouselItemCommandValidator(ILocalizationService _localizationService)
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

    public class CreateCarouselItemCommandHandler : IRequestHandler<CreateCarouselItemCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;
        public CreateCarouselItemCommandHandler(IApplicationDbContext context, ILocalizationService localizationService)
        {
            _context = context;
            _localizationService = localizationService;
        }
        public async Task<Result<int>> Handle(CreateCarouselItemCommand request, CancellationToken cancellationToken)
        {
            var carouselItem = new CarouselItem
            {
                Title = request.Title,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                DisplayOrder = request.DisplayOrder,
                IsActive = true
            };
            _context.CarouselItems.Add(carouselItem);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(carouselItem.Id, _localizationService.GetLocalizedString(LocalizationKeys.CarouselItems.Created));
        }
    }
}
