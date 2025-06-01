using FluentValidation;
using MediatR;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Common.Results;

namespace WebApi.Features.TreatmentFaqs.Commands.Update
{
    public record UpdateTreatmentFaqCommand(
        int Id,
        string Question,
        string Answer,
        int TreatmentId
    ) : IRequest<Result<Unit>>;

    public class UpdateTreatmentFaqCommandValidator : AbstractValidator<UpdateTreatmentFaqCommand>
    {
        public UpdateTreatmentFaqCommandValidator(ILocalizationService localizationService)
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

    public class UpdateTreatmentFaqCommandHandler : IRequestHandler<UpdateTreatmentFaqCommand, Result<Unit>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;

        public UpdateTreatmentFaqCommandHandler(IApplicationDbContext context, ILocalizationService localizationService)
        {
            _context = context;
            _localizationService = localizationService;
        }

        public async Task<Result<Unit>> Handle(UpdateTreatmentFaqCommand request, CancellationToken cancellationToken)
        {
            var treatmentFaq = await _context.TreatmentFaqs.FindAsync(request.Id);
            if (treatmentFaq == null)
            {
                return Result<Unit>.Failure(_localizationService.GetLocalizedString(LocalizationKeys.TreatmentFaqs.NotFound));
            }

            treatmentFaq.Question = request.Question;
            treatmentFaq.Answer = request.Answer;
            treatmentFaq.TreatmentId = request.TreatmentId;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value, _localizationService.GetLocalizedString(LocalizationKeys.TreatmentFaqs.Updated));
        }
    }
}
