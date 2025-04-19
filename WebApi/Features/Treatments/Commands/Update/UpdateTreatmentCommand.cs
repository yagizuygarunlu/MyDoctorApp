using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Infrastructure.Persistence;
using WebApi.Common.Localization;

namespace WebApi.Features.Treatments.Commands.Update
{
    public record UpdateTreatmentCommand(
        int Id,
        string Name,
        string? Description,
        string? Slug,
        List<TreatmentImage>? Images = null
    ) : IRequest<Result<int>>;

    public class UpdateTreatmentCommandValidator : AbstractValidator<UpdateTreatmentCommand>
    {
        public UpdateTreatmentCommandValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Treatments.NameRequired));
        }
    }

    public class UpdateTreatmentCommandHandler : IRequestHandler<UpdateTreatmentCommand, Result<int>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;

        public UpdateTreatmentCommandHandler(ApplicationDbContext context, ILocalizationService localizationService)
        {
            _context = context;
            _localizationService = localizationService;
        }

        public async Task<Result<int>> Handle(UpdateTreatmentCommand request, CancellationToken cancellationToken)
        {
            var treatment = await _context.Treatments.FindAsync(request.Id);
            if (treatment == null)
            {
                return Result<int>.Failure(_localizationService.GetLocalizedString(LocalizationKeys.Treatments.NotFound));
            }

            treatment.Name = request.Name;
            treatment.Description = request.Description;
            treatment.Slug = request.Slug;

            await _context.SaveChangesAsync(cancellationToken);
            await UpdateTreatmentImagesAsync(request.Id, request.Images, cancellationToken);

            return Result<int>.Success(treatment.Id, _localizationService.GetLocalizedString(LocalizationKeys.Treatments.Updated));
        }

        public async Task<Unit> UpdateTreatmentImagesAsync(
            int treatmentId,
            List<TreatmentImage> images,
            CancellationToken cancellationToken)
        {
            var treatmentImages = await _context.TreatmentImages
                .AsNoTracking()
                .Where(x => x.TreatmentId == treatmentId)
                .ToListAsync(cancellationToken);

            _context.TreatmentImages.RemoveRange(treatmentImages);
            await _context.SaveChangesAsync(cancellationToken);
            
            if (images != null && images.Count > 0)
            {
                foreach (var image in images)
                {
                    image.TreatmentId = treatmentId;
                }
                _context.TreatmentImages.AddRange(images);
                await _context.SaveChangesAsync(cancellationToken);
            }
            
            return Unit.Value;
        }
    }
}
