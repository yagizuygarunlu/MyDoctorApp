﻿using MediatR;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Common.Results;

namespace WebApi.Features.CarouselItems.Commands.Delete
{
    public record DeleteCarouselItemCommand(
        int Id
    ) : IRequest<Result<Unit>>;

    public class DeleteCarouselItemCommandHandler : IRequestHandler<DeleteCarouselItemCommand, Result<Unit>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;
        public DeleteCarouselItemCommandHandler(IApplicationDbContext context, ILocalizationService localizationService)
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
            return Result<Unit>.Success(Unit.Value,_localizationService.GetLocalizedString(LocalizationKeys.CarouselItems.Deleted));
        }
    }
}
