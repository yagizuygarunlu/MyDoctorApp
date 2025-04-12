using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.TreatmentFaqs.Queries.GetAll
{
    public record GetAllTreatmentFaqsQuery : IRequest<Result<List<TreatmentFaq>>>;

    public sealed class GetActiveTreatmentFaqsQueryHandler : IRequestHandler<GetAllTreatmentFaqsQuery, Result<List<TreatmentFaq>>>
    {
        private readonly ApplicationDbContext _context;
        public GetActiveTreatmentFaqsQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<List<TreatmentFaq>>> Handle(GetAllTreatmentFaqsQuery request, CancellationToken cancellationToken)
        {
            var treatmentFaqs = await _context.TreatmentFaqs
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return Result<List<TreatmentFaq>>.Success(treatmentFaqs);
        }
    }

}
