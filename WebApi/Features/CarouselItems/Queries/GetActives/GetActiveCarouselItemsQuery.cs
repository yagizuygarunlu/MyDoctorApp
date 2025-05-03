using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.CarouselItems.Queries.GetActives
{
    public record GetActiveCarouselItemsQuery: IRequest<Result<List<CarouselItem>>>;

    public class GetActiveCarouselItemsQueryHandler : IRequestHandler<GetActiveCarouselItemsQuery, Result<List<CarouselItem>>>
    {
        private readonly IApplicationDbContext _context;
        public GetActiveCarouselItemsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<List<CarouselItem>>> Handle(GetActiveCarouselItemsQuery request, CancellationToken cancellationToken)
        {
            var carouselItems = await _context.CarouselItems
                .AsNoTracking()
                .Where(x => x.IsActive)
                .ToListAsync(cancellationToken);
            return Result<List<CarouselItem>>.Success(carouselItems);
        }
    }
}
