using FluentValidation;
using MediatR;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.Treatments.Commands.Create
{
    public record CreateTreatmentCommand(
        string Name,
        string? Description,
        string? Slug
    ) : IRequest<Result<int>>;

    public class CreateTreatmentCommandValidator : AbstractValidator<CreateTreatmentCommand>
    {
        public CreateTreatmentCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");
        }
    }

    public class CreateTreatmentCommandHandler : IRequestHandler<CreateTreatmentCommand, Result<int>>
    {
        private readonly ApplicationDbContext _context;
        public CreateTreatmentCommandHandler(ApplicationDbContext context)
        {
            _context = context;
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
            return Result<int>.Success(treatment.Id);
        }
    }
}
