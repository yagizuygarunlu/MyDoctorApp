using MediatR;
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
        public DeleteCarouselItemCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<Unit>> Handle(DeleteCarouselItemCommand request, CancellationToken cancellationToken)
        {
            var carouselItem = await _context.CarouselItems.FindAsync(request.Id);
            if (carouselItem == null)
            {
                return Result<Unit>.Failure("Carousel item not found.");
            }
            carouselItem.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);
            return Result<Unit>.Success(Unit.Value); // Explicitly pass Unit.Value to match the expected type
        }
    }
}
