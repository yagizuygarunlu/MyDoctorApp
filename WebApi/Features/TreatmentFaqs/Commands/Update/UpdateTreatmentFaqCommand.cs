using FluentValidation;
using MediatR;
using WebApi.Common.Results;
using WebApi.Infrastructure.Persistence;

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
        public UpdateTreatmentFaqCommandValidator()
        {
            RuleFor(x => x.Question)
                .NotEmpty()
                .WithMessage("Question is required.");
            RuleFor(x => x.Answer)
                .NotEmpty()
                .WithMessage("Answer is required.");
            RuleFor(x => x.TreatmentId)
                .NotEmpty()
                .WithMessage("Treatment ID is required.")
                .GreaterThan(0)
                .WithMessage("Treatment ID must be greater than 0.");
        }
    }

    public class UpdateTreatmentFaqCommandHandler : IRequestHandler<UpdateTreatmentFaqCommand, Result<Unit>>
    {
        private readonly ApplicationDbContext _context;
        public UpdateTreatmentFaqCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<Unit>> Handle(UpdateTreatmentFaqCommand request, CancellationToken cancellationToken)
        {
            var treatmentFaq = await _context.TreatmentFaqs.FindAsync(request.Id);
            if (treatmentFaq == null)
            {
                return Result<Unit>.Failure("Treatment FAQ not found.");
            }
            treatmentFaq.Question = request.Question;
            treatmentFaq.Answer = request.Answer;
            treatmentFaq.TreatmentId = request.TreatmentId;
            await _context.SaveChangesAsync(cancellationToken);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
