using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.Reviews.Commands
{
    public record CreateReviewCommand(
        int DoctorId,
        string Name,
        string Message,
        int Rating
    ) : IRequest<Result<int>>;

    public class CreateReviewValidator : AbstractValidator<CreateReviewCommand>
    {
        public CreateReviewValidator()
        {
            RuleFor(x => x.DoctorId)
                .GreaterThan(0).WithMessage("DoctorId must be greater than zero.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required.")
                .MaximumLength(1000);

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
        }
    }

    public sealed class CreateReviewHandler : IRequestHandler<CreateReviewCommand, Result<int>>
    {
        private readonly ApplicationDbContext _dbContext;

        public CreateReviewHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<Result<int>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            var review = new Review
            {
                DoctorId = request.DoctorId,
                Name = request.Name,
                Message = request.Message,
                Rating = request.Rating,
                CreatedAt = DateTimeOffset.UtcNow,
                IsApproved = false
            };

            _dbContext.Reviews.Add(review);
            var saveResult = await _dbContext.SaveChangesAsync(cancellationToken);

            if (saveResult > 0)
                return Result<int>.Success(review.Id);

            return Result<int>.Failure("Review could not be created.");
        }
    }
}