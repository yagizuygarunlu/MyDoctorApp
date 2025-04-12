using FluentValidation;
using MediatR;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.TreatmentFaqs.Commands.Create
{
    public record CreateTreatmentFaqCommand(
        string Question,
        string Answer,
        int TreatmentId
    ) : IRequest<Result<int>>;

    public class CreateTreatmentFaqCommandValidator : AbstractValidator<CreateTreatmentFaqCommand>
    {
        public CreateTreatmentFaqCommandValidator()
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

    public class CreateTreatmentFaqCommandHandler : IRequestHandler<CreateTreatmentFaqCommand, Result<int>>
    {
        private readonly ApplicationDbContext _context;
        public CreateTreatmentFaqCommandHandler(ApplicationDbContext context)
        {
            _context = context;
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
            await _context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(treatmentFaq.Id);
        }
    }
}
