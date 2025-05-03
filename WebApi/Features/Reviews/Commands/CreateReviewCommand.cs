using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Infrastructure.Persistence;
using WebApi.Common.Localization;
using WebApi.Application.Common.Interfaces;

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
        public CreateReviewValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.DoctorId)
                .GreaterThan(0)
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Reviews.DoctorIdRequired));

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Reviews.NameRequired))
                .MaximumLength(100);

            RuleFor(x => x.Message)
                .NotEmpty()
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Reviews.MessageRequired))
                .MaximumLength(1000);

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5)
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Reviews.InvalidRating));
        }
    }

    public sealed class CreateReviewHandler : IRequestHandler<CreateReviewCommand, Result<int>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILocalizationService _localizationService;

        public CreateReviewHandler(ApplicationDbContext dbContext, ILocalizationService localizationService)
        {
            _dbContext = dbContext;
            _localizationService = localizationService;
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
                return Result<int>.Success(review.Id, _localizationService.GetLocalizedString(LocalizationKeys.Reviews.Created));

            return Result<int>.Failure(_localizationService.GetLocalizedString(LocalizationKeys.Reviews.CreationFailed));
        }
    }
}