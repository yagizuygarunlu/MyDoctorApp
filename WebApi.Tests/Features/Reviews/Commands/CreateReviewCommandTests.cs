using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Domain.Entities;
using WebApi.Features.Reviews.Commands;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Tests.Features.Reviews.Commands
{
    public class CreateReviewValidatorTests
    {
        private readonly ILocalizationService _localizationService;
        private readonly CreateReviewValidator _validator;

        public CreateReviewValidatorTests()
        {
            _localizationService = Substitute.For<ILocalizationService>();
            _localizationService.GetLocalizedString(Arg.Any<string>()).Returns(callInfo => callInfo.Arg<string>());
            _validator = new CreateReviewValidator(_localizationService);
        }

        [Fact]
        public void Should_Have_Error_When_DoctorId_Is_Zero()
        {
            var command = new CreateReviewCommand(0, "John", "Great doctor", 5);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.DoctorId);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            var command = new CreateReviewCommand(1, "", "Great doctor", 5);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Too_Long()
        {
            var command = new CreateReviewCommand(1, new string('a', 101), "Great doctor", 5);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Message_Is_Empty()
        {
            var command = new CreateReviewCommand(1, "John", "", 5);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Message);
        }

        [Fact]
        public void Should_Have_Error_When_Message_Too_Long()
        {
            var command = new CreateReviewCommand(1, "John", new string('a', 1001), 5);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public void Should_Have_Error_When_Rating_Is_Out_Of_Range(int rating)
        {
            var command = new CreateReviewCommand(1, "John", "Great doctor", rating);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Rating);
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            var command = new CreateReviewCommand(1, "John", "Great doctor", 5);
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    public class CreateReviewHandlerTests
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILocalizationService _localizationService;
        private readonly CreateReviewHandler _handler;
        private readonly List<Review> _reviews;

        public CreateReviewHandlerTests()
        {
            _dbContext = Substitute.For<IApplicationDbContext>();
            _localizationService = Substitute.For<ILocalizationService>();
            _reviews = new List<Review>();

            // Setup Reviews DbSet
            var reviewsDbSet = Substitute.For<DbSet<Review>, IQueryable<Review>>();
            reviewsDbSet.Add(Arg.Do<Review>(r =>
            {
                // Simulate EF Core setting the Id after save
                r.Id = new Random().Next(1, 10000);
                _reviews.Add(r);
            }));

            _dbContext.Reviews.Returns(reviewsDbSet);

            _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>())
                .Returns(ci => _reviews.Count > 0 ? 1 : 0);

            _localizationService.GetLocalizedString(Arg.Any<string>())
                .Returns(callInfo => callInfo.Arg<string>());

            // Use ApplicationDbContext for constructor, but substitute for interface
            _handler = new CreateReviewHandler((ApplicationDbContext)_dbContext, _localizationService);
        }

        [Fact]
        public async Task Handle_Should_Add_Review_And_Return_Success_When_Save_Succeeds()
        {
            var command = new CreateReviewCommand(1, "Jane", "Excellent!", 5);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();
            result.Data.ShouldBeGreaterThan(0);
            result.Message.ShouldBe("Reviews.Created");

            _reviews.Count.ShouldBe(1);
            var review = _reviews[0];
            review.DoctorId.ShouldBe(1);
            review.Name.ShouldBe("Jane");
            review.Message.ShouldBe("Excellent!");
            review.Rating.ShouldBe(5);
            review.IsApproved.ShouldBeFalse();
            review.CreatedAt.ShouldBeInRange(DateTimeOffset.UtcNow.AddMinutes(-1), DateTimeOffset.UtcNow.AddMinutes(1));
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_When_Save_Fails()
        {
            // Simulate SaveChangesAsync returns 0 (no rows affected)
            _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(0);

            var command = new CreateReviewCommand(1, "Jane", "Excellent!", 5);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeFalse();
            result.Error.ShouldBe("Reviews.CreationFailed");
        }
    }
}
