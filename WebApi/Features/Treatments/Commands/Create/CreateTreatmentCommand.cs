using FluentValidation;
using MediatR;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Common.Results;
using WebApi.Domain.Entities;

namespace WebApi.Features.Treatments.Commands.Create
{
    public record CreateTreatmentCommand(
        string Name,
        string? Description,
        string? Slug,
        List<TreatmentImage>? Images = null
    ) : IRequest<Result<int>>;

    public class CreateTreatmentCommandValidator : AbstractValidator<CreateTreatmentCommand>
    {
        public CreateTreatmentCommandValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Treatments.NameRequired));
        }
    }

    public class CreateTreatmentCommandHandler : IRequestHandler<CreateTreatmentCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;

        public CreateTreatmentCommandHandler(IApplicationDbContext context, ILocalizationService localizationService)
        {
            _context = context;
            _localizationService = localizationService;
        }

        public async Task<Result<int>> Handle(CreateTreatmentCommand request, CancellationToken cancellationToken)
        {
            var treatment = new Treatment
            {
                Name = request.Name,
                Description = request.Description,
                Slug = request.Slug
            };

            _context.Treatments.Add(treatment);
            await _context.SaveChangesAsync(cancellationToken);
            await AddTreatmentImagesAsync(request.Images, treatment.Id, cancellationToken);

            return Result<int>.Success(treatment.Id, _localizationService.GetLocalizedString(LocalizationKeys.Treatments.Created));
        }

        public async Task<Unit> AddTreatmentImagesAsync(
            List<TreatmentImage>? images,
            int treatmentId,
            CancellationToken cancellationToken)
        {
            if (images == null || images.Count == 0)
                return Unit.Value;

            foreach (var image in images)
            {
                image.TreatmentId = treatmentId;
            }

            _context.TreatmentImages.AddRange(images);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
