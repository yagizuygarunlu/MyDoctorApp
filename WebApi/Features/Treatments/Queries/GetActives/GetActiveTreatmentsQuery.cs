﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.Treatments.Queries.GetActives
{
    public record GetActiveTreatmentsQuery: IRequest<Result<List<Treatment>>>;

    public class GetActiveTreatmentsQueryHandler : IRequestHandler<GetActiveTreatmentsQuery, Result<List<Treatment>>>
    {
        private readonly IApplicationDbContext _context;
        public GetActiveTreatmentsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<List<Treatment>>> Handle(GetActiveTreatmentsQuery request, CancellationToken cancellationToken)
        {
            var treatments = await _context.Treatments
                .Include(x => x.Images)
                .AsNoTracking()
                .Where(x => x.IsActive)
                .ToListAsync(cancellationToken);
            return Result<List<Treatment>>.Success(treatments);
        }
    }
}
