using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.CarouselItems.Queries.GetAll
{
    public record GetAllCarouselItemsQuery: IRequest<Result<List<CarouselItem>>>;

    public class GetAllCarouselItemsQueryHandler : IRequestHandler<GetAllCarouselItemsQuery, Result<List<CarouselItem>>>
    {
        private readonly IApplicationDbContext _context;
        public GetAllCarouselItemsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<List<CarouselItem>>> Handle(GetAllCarouselItemsQuery request, CancellationToken cancellationToken)
        {
            var carouselItems = await _context.CarouselItems
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Result<List<CarouselItem>>.Success(carouselItems);
        }
    }
}
