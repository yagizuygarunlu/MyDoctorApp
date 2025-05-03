using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.Treatments.Queries.GetAll
{
    public record GetAllTreatmentsQuery: IRequest<Result<List<Treatment>>>;

    public class GetAllTreatmentsQueryHandler : IRequestHandler<GetAllTreatmentsQuery, Result<List<Treatment>>>
    {
        private readonly IApplicationDbContext _context;
        public GetAllTreatmentsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<List<Treatment>>> Handle(GetAllTreatmentsQuery request, CancellationToken cancellationToken)
        {
            var treatments = await _context.Treatments
                .Include(x => x.Images)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return Result<List<Treatment>>.Success(treatments);
        }
    }
}
