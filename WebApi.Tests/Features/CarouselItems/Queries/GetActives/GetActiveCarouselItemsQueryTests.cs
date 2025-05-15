using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Domain.Entities;
using WebApi.Features.CarouselItems.Queries.GetActives;

namespace WebApi.Tests.Features.CarouselItems.Queries.GetActives
{
    public class GetActiveCarouselItemsQueryTests
    {
        public class GetActiveCarouselItemsQueryHandlerTests
        {
            private readonly IApplicationDbContext _context;
            private readonly GetActiveCarouselItemsQueryHandler _handler;

            public GetActiveCarouselItemsQueryHandlerTests()
            {
                // Arrange
                _context = Substitute.For<IApplicationDbContext>();
                _handler = new GetActiveCarouselItemsQueryHandler(_context);
            }

            [Fact]
            public async Task Handle_ShouldReturnOnlyActiveCarouselItems()
            {
                // Arrange
                var query = new GetActiveCarouselItemsQuery();
                var carouselItems = new List<CarouselItem>
                {
                    new CarouselItem { Id = 1, Title = "Active Item 1", ImageUrl = "image1.jpg", IsActive = true },
                    new CarouselItem { Id = 2, Title = "Inactive Item", ImageUrl = "image2.jpg", IsActive = false },
                    new CarouselItem { Id = 3, Title = "Active Item 2", ImageUrl = "image3.jpg", IsActive = true }
                }.AsQueryable();

                var mockDbSet = carouselItems.BuildMockDbSet();
                _context.CarouselItems.Returns(mockDbSet);

                // Act
                var result = await _handler.Handle(query, CancellationToken.None);

                // Assert
                result.Succeeded.ShouldBeTrue();
                result.IsSuccess.ShouldBeTrue();
                result.Data.ShouldNotBeNull();
                result.Data.Count.ShouldBe(2);
                result.Data.All(item => item.IsActive).ShouldBeTrue();
                result.Data.ShouldContain(item => item.Id == 1);
                result.Data.ShouldContain(item => item.Id == 3);
                result.Data.ShouldNotContain(item => item.Id == 2);
            }

            [Fact]
            public async Task Handle_WhenNoActiveCarouselItems_ShouldReturnEmptyList()
            {
                // Arrange
                var query = new GetActiveCarouselItemsQuery();
                var carouselItems = new List<CarouselItem>
                {
                    new CarouselItem { Id = 1, Title = "Inactive Item 1", ImageUrl = "image1.jpg", IsActive = false },
                    new CarouselItem { Id = 2, Title = "Inactive Item 2", ImageUrl = "image2.jpg", IsActive = false }
                }.AsQueryable();

                var mockDbSet = carouselItems.BuildMockDbSet();
                _context.CarouselItems.Returns(mockDbSet);

                // Act
                var result = await _handler.Handle(query, CancellationToken.None);

                // Assert
                result.Succeeded.ShouldBeTrue();
                result.IsSuccess.ShouldBeTrue();
                result.Data.ShouldNotBeNull();
                result.Data.Count.ShouldBe(0);
            }

            [Fact]
            public async Task Handle_WhenNoCarouselItemsExist_ShouldReturnEmptyList()
            {
                // Arrange
                var query = new GetActiveCarouselItemsQuery();
                var emptyList = new List<CarouselItem>().AsQueryable();

                var mockDbSet = emptyList.BuildMockDbSet();
                _context.CarouselItems.Returns(mockDbSet);

                // Act
                var result = await _handler.Handle(query, CancellationToken.None);

                // Assert
                result.Succeeded.ShouldBeTrue();
                result.IsSuccess.ShouldBeTrue();
                result.Data.ShouldNotBeNull();
                result.Data.Count.ShouldBe(0);
            }

            [Fact]
            public async Task Handle_WhenExceptionOccurs_ShouldPropagateException()
            {
                // Arrange
                var query = new GetActiveCarouselItemsQuery();

                // Mock the DbContext to throw an exception when CarouselItems is accessed
                var dbUpdateException = new DbUpdateException("Database error occurred");
                _context.CarouselItems.Returns(x => { throw dbUpdateException; });

                // Act & Assert
                var exception = await Should.ThrowAsync<DbUpdateException>(async () =>
                    await _handler.Handle(query, CancellationToken.None)
                );

                exception.ShouldBe(dbUpdateException);
            }
        }
    }
}
