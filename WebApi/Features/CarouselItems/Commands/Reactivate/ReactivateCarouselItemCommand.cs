using MediatR;
using WebApi.Common.Results;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.CarouselItems.Commands.Reactivate
{
    public record ReactivateCarouselItemCommand(
        int Id
    ) : IRequest<Result<int>>;

    public class ReactivateCarouselItemCommandHandler : IRequestHandler<ReactivateCarouselItemCommand, Result<int>>
    {
        private readonly ApplicationDbContext _context;
        public ReactivateCarouselItemCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<int>> Handle(ReactivateCarouselItemCommand request, CancellationToken cancellationToken)
        {
            var carouselItem = await _context.CarouselItems.FindAsync(request.Id);
            if (carouselItem == null)
            {
                return Result<int>.Failure("Carousel item not found.");
            }
            carouselItem.IsActive = true;
            await _context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(carouselItem.Id);
        }
    }
}
