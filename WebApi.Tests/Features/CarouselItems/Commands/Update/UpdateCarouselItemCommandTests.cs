using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Domain.Entities;
using WebApi.Features.CarouselItems.Commands.Update;

namespace WebApi.Tests.Features.CarouselItems.Commands.Update
{
    public class UpdateCarouselItemCommandTests
    {
        public class UpdateCarouselItemCommandHandlerTests
        {
            private readonly IApplicationDbContext _context;
            private readonly ILocalizationService _localizationService;
            private readonly UpdateCarouselItemCommandHandler _handler;

            public UpdateCarouselItemCommandHandlerTests()
            {
                _context = Substitute.For<IApplicationDbContext>();
                _localizationService = Substitute.For<ILocalizationService>();
                _handler = new UpdateCarouselItemCommandHandler(_context, _localizationService);
            }

            [Fact]
            public async Task Handle_ShouldUpdateCarouselItem_AndReturnSuccess_WhenItemExists()
            {
                // Arrange
                var carouselItem = new CarouselItem
                {
                    Id = 1,
                    Title = "Old Title",
                    Description = "Old Desc",
                    ImageUrl = "old.jpg",
                    DisplayOrder = 1,
                    IsActive = true
                };
                var dbSet = Substitute.For<DbSet<CarouselItem>>();
                dbSet.FindAsync(1).Returns(new ValueTask<CarouselItem>(carouselItem));
                _context.CarouselItems.Returns(dbSet);

                _context.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
                _localizationService.GetLocalizedString(Arg.Any<string>()).Returns("Updated");

                var command = new UpdateCarouselItemCommand(1, "New Title", "New Desc", "new.jpg", 2);

                // Act
                var result = await _handler.Handle(command, CancellationToken.None);

                // Assert
                carouselItem.Title.ShouldBe("New Title");
                carouselItem.Description.ShouldBe("New Desc");
                carouselItem.ImageUrl.ShouldBe("new.jpg");
                carouselItem.DisplayOrder.ShouldBe(2);
                await _context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
                result.Succeeded.ShouldBeTrue();
                result.IsSuccess.ShouldBeTrue();
                result.Data.ShouldBe(1);
                result.Message.ShouldBe("Updated");
            }

            [Fact]
            public async Task Handle_ShouldReturnFailure_WhenItemDoesNotExist()
            {
                // Arrange
                var dbSet = Substitute.For<DbSet<CarouselItem>>();
                dbSet.FindAsync(2).Returns(new ValueTask<CarouselItem>((CarouselItem)null));
                _context.CarouselItems.Returns(dbSet);

                _localizationService.GetLocalizedString(Arg.Any<string>()).Returns("Not found");

                var command = new UpdateCarouselItemCommand(2, "Title", "Desc", "img.jpg", 1);

                // Act
                var result = await _handler.Handle(command, CancellationToken.None);

                // Assert
                result.Succeeded.ShouldBeFalse();
                result.IsSuccess.ShouldBeFalse();
                result.Error.ShouldBe("Not found");
            }

            [Fact]
            public async Task Handle_WhenSaveChangesThrows_ShouldPropagateException()
            {
                // Arrange
                var carouselItem = new CarouselItem
                {
                    Id = 3,
                    Title = "T",
                    Description = "D",
                    ImageUrl = "i.jpg",
                    DisplayOrder = 1,
                    IsActive = true
                };
                var dbSet = Substitute.For<DbSet<CarouselItem>>();
                dbSet.FindAsync(3).Returns(new ValueTask<CarouselItem>(carouselItem));
                _context.CarouselItems.Returns(dbSet);

                _context.SaveChangesAsync(Arg.Any<CancellationToken>())
                    .Returns<Task<int>>(x => { throw new DbUpdateException("DB error"); });

                var command = new UpdateCarouselItemCommand(3, "T", "D", "i.jpg", 1);

                // Act & Assert
                await Should.ThrowAsync<DbUpdateException>(async () =>
                    await _handler.Handle(command, CancellationToken.None)
                );
            }
        }

        public class UpdateCarouselItemCommandValidatorTests
        {
            private readonly ILocalizationService _localizationService;
            private readonly UpdateCarouselItemCommandValidator _validator;

            public UpdateCarouselItemCommandValidatorTests()
            {
                _localizationService = Substitute.For<ILocalizationService>();
                _localizationService.GetLocalizedString(Arg.Any<string>()).Returns("Localized error");
                _validator = new UpdateCarouselItemCommandValidator(_localizationService);
            }

            [Fact]
            public void Should_Have_Error_When_Title_Is_Empty()
            {
                var command = new UpdateCarouselItemCommand(1, "", "desc", "img", 1);
                var result = _validator.TestValidate(command);
                result.ShouldHaveValidationErrorFor(x => x.Title)
                    .WithErrorMessage("Localized error");
            }

            [Fact]
            public void Should_Have_Error_When_ImageUrl_Is_Empty()
            {
                var command = new UpdateCarouselItemCommand(1, "title", "desc", "", 1);
                var result = _validator.TestValidate(command);
                result.ShouldHaveValidationErrorFor(x => x.ImageUrl)
                    .WithErrorMessage("Localized error");
            }

            [Fact]
            public void Should_Have_Error_When_DisplayOrder_Is_Not_Greater_Than_Zero()
            {
                var command = new UpdateCarouselItemCommand(1, "title", "desc", "img", 0);
                var result = _validator.TestValidate(command);
                result.ShouldHaveValidationErrorFor(x => x.DisplayOrder)
                    .WithErrorMessage("Localized error");
            }

            [Fact]
            public void Should_Not_Have_Error_For_Valid_Command()
            {
                var command = new UpdateCarouselItemCommand(1, "title", "desc", "img", 1);
                var result = _validator.TestValidate(command);
                result.ShouldNotHaveAnyValidationErrors();
            }
        }
    }
}
