using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Domain.Entities;
using WebApi.Features.CarouselItems.Commands.Reactivate;

namespace WebApi.Tests.Features.CarouselItems.Commands.Reactivate
{
    public class ReactivateCarouselItemCommandTests
    {
        public class ReactivateCarouselItemCommandHandlerTests
        {
            private readonly IApplicationDbContext _context;
            private readonly ILocalizationService _localizationService;
            private readonly ReactivateCarouselItemCommandHandler _handler;

            public ReactivateCarouselItemCommandHandlerTests()
            {
                _context = Substitute.For<IApplicationDbContext>();
                _localizationService = Substitute.For<ILocalizationService>();
                _handler = new ReactivateCarouselItemCommandHandler(_context, _localizationService);
            }

            [Fact]
            public async Task Handle_ShouldSetIsActiveTrue_AndReturnSuccess_WhenItemExists()
            {
                // Arrange
                var carouselItem = new CarouselItem
                {
                    Id = 1,
                    Title = "Test",
                    ImageUrl = "img",
                    IsActive = false
                };
                var dbSet = Substitute.For<DbSet<CarouselItem>>();
                dbSet.FindAsync(1).Returns(new ValueTask<CarouselItem>(carouselItem));
                _context.CarouselItems.Returns(dbSet);

                _context.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
                _localizationService.GetLocalizedString(Arg.Any<string>()).Returns("Reactivated");

                var command = new ReactivateCarouselItemCommand(1);

                // Act
                var result = await _handler.Handle(command, CancellationToken.None);

                // Assert
                carouselItem.IsActive.ShouldBeTrue();
                await _context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
                result.Succeeded.ShouldBeTrue();
                result.IsSuccess.ShouldBeTrue();
                result.Data.ShouldBe(1);
                result.Message.ShouldBe("Reactivated");
            }

            [Fact]
            public async Task Handle_ShouldReturnFailure_WhenItemDoesNotExist()
            {
                // Arrange
                var dbSet = Substitute.For<DbSet<CarouselItem>>();
                dbSet.FindAsync(2).Returns(new ValueTask<CarouselItem>((CarouselItem)null));
                _context.CarouselItems.Returns(dbSet);

                _localizationService.GetLocalizedString(Arg.Any<string>()).Returns("Not found");

                var command = new ReactivateCarouselItemCommand(2);

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
                    Title = "Test",
                    ImageUrl = "img",
                    IsActive = false
                };
                var dbSet = Substitute.For<DbSet<CarouselItem>>();
                dbSet.FindAsync(3).Returns(new ValueTask<CarouselItem>(carouselItem));
                _context.CarouselItems.Returns(dbSet);

                _context.SaveChangesAsync(Arg.Any<CancellationToken>())
                    .Returns<Task<int>>(x => { throw new DbUpdateException("DB error"); });

                var command = new ReactivateCarouselItemCommand(3);

                // Act & Assert
                await Should.ThrowAsync<DbUpdateException>(async () =>
                    await _handler.Handle(command, CancellationToken.None)
                );
            }
        }
    }
}
