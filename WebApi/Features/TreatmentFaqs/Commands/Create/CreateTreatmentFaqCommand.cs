using FluentValidation;
using MediatR;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Infrastructure.Persistence;
using WebApi.Common.Localization;
using WebApi.Application.Common.Interfaces;

namespace WebApi.Features.TreatmentFaqs.Commands.Create
{
    public record CreateTreatmentFaqCommand(
        string Question,
        string Answer,
        int TreatmentId
    ) : IRequest<Result<int>>;

    public class CreateTreatmentFaqCommandValidator : AbstractValidator<CreateTreatmentFaqCommand>
    {
        public CreateTreatmentFaqCommandValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Question)
                .NotEmpty()
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.TreatmentFaqs.QuestionRequired));
            RuleFor(x => x.Answer)
                .NotEmpty()
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.TreatmentFaqs.AnswerRequired));
            RuleFor(x => x.TreatmentId)
                .NotEmpty()
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.TreatmentFaqs.TreatmentIdRequired))
                .GreaterThan(0)
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.TreatmentFaqs.InvalidTreatmentId));
        }
    }

    public class CreateTreatmentFaqCommandHandler : IRequestHandler<CreateTreatmentFaqCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;

        public CreateTreatmentFaqCommandHandler(IApplicationDbContext context, ILocalizationService localizationService)
        {
            _context = context;
            _localizationService = localizationService;
        }

        public async Task<Result<int>> Handle(CreateTreatmentFaqCommand request, CancellationToken cancellationToken)
        {
            var treatmentFaq = new TreatmentFaq
            {
                Question = request.Question,
                Answer = request.Answer,
                TreatmentId = request.TreatmentId
            };

            _context.TreatmentFaqs.Add(treatmentFaq);
            var saveResult = await _context.SaveChangesAsync(cancellationToken);

            if (saveResult > 0)
                return Result<int>.Success(treatmentFaq.Id, _localizationService.GetLocalizedString(LocalizationKeys.TreatmentFaqs.Created));

            return Result<int>.Failure(_localizationService.GetLocalizedString(LocalizationKeys.TreatmentFaqs.CreationFailed));
        }
    }
}
