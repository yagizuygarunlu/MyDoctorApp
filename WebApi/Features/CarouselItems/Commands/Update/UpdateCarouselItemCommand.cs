using FluentValidation;
using MediatR;
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
        public UpdateCarouselItemCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.");
            RuleFor(x => x.ImageUrl)
                .NotEmpty()
                .WithMessage("Image URL is required.");
            RuleFor(x => x.DisplayOrder)
                .GreaterThan(0)
                .WithMessage("Display order must be greater than 0.");
        }
    }

    public class UpdateCarouselItemCommandHandler : IRequestHandler<UpdateCarouselItemCommand, Result<int>>
    {
        private readonly ApplicationDbContext _context;
        public UpdateCarouselItemCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<int>> Handle(UpdateCarouselItemCommand request, CancellationToken cancellationToken)
        {
            var carouselItem = await _context.CarouselItems.FindAsync(request.Id);
            if (carouselItem == null)
            {
                return Result<int>.Failure("Carousel item not found.");
            }
            carouselItem.Title = request.Title;
            carouselItem.Description = request.Description;
            carouselItem.ImageUrl = request.ImageUrl;
            carouselItem.DisplayOrder = request.DisplayOrder;
            await _context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(carouselItem.Id);
        }
    }
}
