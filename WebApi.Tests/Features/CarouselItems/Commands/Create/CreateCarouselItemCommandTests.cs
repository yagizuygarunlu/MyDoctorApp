using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Domain.Entities;
using WebApi.Features.CarouselItems.Commands.Create;

namespace WebApi.Tests.Features.CarouselItems.Commands.Create
{
    public class CreateCarouselItemCommandTests
    {
        public class CreateCarouselItemCommandHandlerTests
        {
            private readonly IApplicationDbContext _context;
            private readonly ILocalizationService _localizationService;
            private readonly CreateCarouselItemCommandHandler _handler;

            public CreateCarouselItemCommandHandlerTests()
            {
                _context = Substitute.For<IApplicationDbContext>();
                _localizationService = Substitute.For<ILocalizationService>();
                _handler = new CreateCarouselItemCommandHandler(_context, _localizationService);
            }

            [Fact]
            public async Task Handle_ShouldAddCarouselItem_AndReturnSuccessResult()
            {
                // Arrange
                var command = new CreateCarouselItemCommand(
                    "Test Title",
                    "Test Description",
                    "http://image.url",
                    1
                );

                var dbSet = Substitute.For<DbSet<CarouselItem>, IQueryable<CarouselItem>>();
                _context.CarouselItems.Returns(dbSet);

                _context.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

                _localizationService.GetLocalizedString(Arg.Any<string>()).Returns("Created");

                // Act
                var result = await _handler.Handle(command, CancellationToken.None);

                // Assert
                await _context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
                dbSet.Received(1).Add(Arg.Is<CarouselItem>(c =>
                    c.Title == "Test Title" &&
                    c.Description == "Test Description" &&
                    c.ImageUrl == "http://image.url" &&
                    c.DisplayOrder == 1 &&
                    c.IsActive
                ));
                result.Succeeded.ShouldBeTrue();
                result.IsSuccess.ShouldBeTrue();
                result.Data.ShouldBeOfType<int>();
                result.Message.ShouldBe("Created");
            }

            [Fact]
            public async Task Handle_WhenSaveChangesThrows_ShouldPropagateException()
            {
                // Arrange
                var command = new CreateCarouselItemCommand(
                    "Test Title",
                    "Test Description",
                    "http://image.url",
                    1
                );

                var dbSet = Substitute.For<DbSet<CarouselItem>, IQueryable<CarouselItem>>();
                _context.CarouselItems.Returns(dbSet);

                _context.SaveChangesAsync(Arg.Any<CancellationToken>())
                    .Returns<Task<int>>(x => { throw new DbUpdateException("DB error"); });

                // Act & Assert
                await Should.ThrowAsync<DbUpdateException>(async () =>
                    await _handler.Handle(command, CancellationToken.None)
                );
            }
        }

        public class CreateCarouselItemCommandValidatorTests
        {
            private readonly ILocalizationService _localizationService;
            private readonly CreateCarouselItemCommandValidator _validator;

            public CreateCarouselItemCommandValidatorTests()
            {
                _localizationService = Substitute.For<ILocalizationService>();
                _localizationService.GetLocalizedString(Arg.Any<string>()).Returns("Localized error");
                _validator = new CreateCarouselItemCommandValidator(_localizationService);
            }

            [Fact]
            public void Should_Have_Error_When_Title_Is_Empty()
            {
                var command = new CreateCarouselItemCommand(
                    "", "desc", "img", 1
                );
                var result = _validator.TestValidate(command);
                result.ShouldHaveValidationErrorFor(x => x.Title)
                    .WithErrorMessage("Localized error");
            }

            [Fact]
            public void Should_Have_Error_When_ImageUrl_Is_Empty()
            {
                var command = new CreateCarouselItemCommand(
                    "title", "desc", "", 1
                );
                var result = _validator.TestValidate(command);
                result.ShouldHaveValidationErrorFor(x => x.ImageUrl)
                    .WithErrorMessage("Localized error");
            }

            [Fact]
            public void Should_Have_Error_When_DisplayOrder_Is_Not_Greater_Than_Zero()
            {
                var command = new CreateCarouselItemCommand(
                    "title", "desc", "img", 0
                );
                var result = _validator.TestValidate(command);
                result.ShouldHaveValidationErrorFor(x => x.DisplayOrder)
                    .WithErrorMessage("Localized error");
            }

            [Fact]
            public void Should_Not_Have_Error_For_Valid_Command()
            {
                var command = new CreateCarouselItemCommand(
                    "title", "desc", "img", 1
                );
                var result = _validator.TestValidate(command);
                result.ShouldNotHaveAnyValidationErrors();
            }
        }
    }
}
