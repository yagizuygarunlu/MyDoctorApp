using FluentValidation;
using MediatR;
using WebApi.Common.Results;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.Treatments.Commands.Update
{
    public record UpdateTreatmentCommand(
        int Id,
        string Name,
        string? Description,
        string? Slug
    ) : IRequest<Result<int>>;

    public class UpdateTreatmentCommandValidator : AbstractValidator<UpdateTreatmentCommand>
    {
        public UpdateTreatmentCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");
        }
    }

    public class UpdateTreatmentCommandHandler : IRequestHandler<UpdateTreatmentCommand, Result<int>>
    {
        private readonly ApplicationDbContext _context;
        public UpdateTreatmentCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<int>> Handle(UpdateTreatmentCommand request, CancellationToken cancellationToken)
        {
            var treatment = await _context.Treatments.FindAsync(request.Id);
            if (treatment == null)
            {
                return Result<int>.Failure("Treatment not found.");
            }
            treatment.Name = request.Name;
            treatment.Description = request.Description;
            treatment.Slug = request.Slug;
            await _context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(treatment.Id);
        }
    }
}
