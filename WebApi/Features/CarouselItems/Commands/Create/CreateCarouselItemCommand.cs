using FluentValidation;
using MediatR;
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
        public CreateCarouselItemCommandValidator()
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

    public class CreateCarouselItemCommandHandler : IRequestHandler<CreateCarouselItemCommand, Result<int>>
    {
        private readonly ApplicationDbContext _context;
        public CreateCarouselItemCommandHandler(ApplicationDbContext context)
        {
            _context = context;
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
            return Result<int>.Success(carouselItem.Id);
        }
    }
}
