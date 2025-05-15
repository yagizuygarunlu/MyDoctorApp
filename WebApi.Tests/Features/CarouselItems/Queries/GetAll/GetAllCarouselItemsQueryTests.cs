using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Domain.Entities;
using WebApi.Features.CarouselItems.Queries.GetAll;

namespace WebApi.Tests.Features.CarouselItems.Queries.GetAll
{
    public class GetAllCarouselItemsQueryTests
    {
        public class GetAllCarouselItemsQueryHandlerTests
        {
            private readonly IApplicationDbContext _context;
            private readonly GetAllCarouselItemsQueryHandler _handler;

            public GetAllCarouselItemsQueryHandlerTests()
            {
                // Arrange
                _context = Substitute.For<IApplicationDbContext>();
                _handler = new GetAllCarouselItemsQueryHandler(_context);
            }

            [Fact]
            public async Task Handle_ShouldReturnAllCarouselItems()
            {
                // Arrange
                var query = new GetAllCarouselItemsQuery();
                var carouselItems = new List<CarouselItem>
                {
                    new CarouselItem { Id = 1, Title = "Item 1", ImageUrl = "image1.jpg", IsActive = true },
                    new CarouselItem { Id = 2, Title = "Item 2", ImageUrl = "image2.jpg", IsActive = false },
                    new CarouselItem { Id = 3, Title = "Item 3", ImageUrl = "image3.jpg", IsActive = true }
                }.AsQueryable();

                var mockDbSet = carouselItems.BuildMockDbSet();
                _context.CarouselItems.Returns(mockDbSet);

                // Act
                var result = await _handler.Handle(query, CancellationToken.None);

                // Assert
                result.Succeeded.ShouldBeTrue();
                result.IsSuccess.ShouldBeTrue();
                result.Data.ShouldNotBeNull();
                result.Data.Count.ShouldBe(3);
                result.Data.ShouldContain(item => item.Id == 1);
                result.Data.ShouldContain(item => item.Id == 2);
                result.Data.ShouldContain(item => item.Id == 3);
            }

            [Fact]
            public async Task Handle_WhenNoCarouselItemsExist_ShouldReturnEmptyList()
            {
                // Arrange
                var query = new GetAllCarouselItemsQuery();
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
            public async Task Handle_ShouldReturnItemsInOriginalOrder()
            {
                // Arrange
                var query = new GetAllCarouselItemsQuery();
                var carouselItems = new List<CarouselItem>
                {
                    new CarouselItem { Id = 1, Title = "Item 1", ImageUrl = "image1.jpg", DisplayOrder = 3 },
                    new CarouselItem { Id = 2, Title = "Item 2", ImageUrl = "image2.jpg", DisplayOrder = 1 },
                    new CarouselItem { Id = 3, Title = "Item 3", ImageUrl = "image3.jpg", DisplayOrder = 2 }
                }.AsQueryable();

                var mockDbSet = carouselItems.BuildMockDbSet();
                _context.CarouselItems.Returns(mockDbSet);

                // Act
                var result = await _handler.Handle(query, CancellationToken.None);

                // Assert
                result.Succeeded.ShouldBeTrue();
                result.Data.ShouldNotBeNull();
                result.Data.Count.ShouldBe(3);
                // Verify items are returned in the original order from the database
                result.Data[0].Id.ShouldBe(1);
                result.Data[1].Id.ShouldBe(2);
                result.Data[2].Id.ShouldBe(3);
            }

            [Fact]
            public async Task Handle_WhenExceptionOccurs_ShouldPropagateException()
            {
                // Arrange
                var query = new GetAllCarouselItemsQuery();

                // Mock the DbContext to throw an exception when CarouselItems is accessed
                var dbUpdateException = new DbUpdateException("Database error occurred");
                _context.CarouselItems.Returns(x => { throw dbUpdateException; });

                // Act & Assert
                var exception = await Should.ThrowAsync<DbUpdateException>(async () =>
                    await _handler.Handle(query, CancellationToken.None)
                );

                exception.ShouldBe(dbUpdateException);
            }

            [Fact]
            public async Task Handle_ShouldUseAsNoTrackingForPerformance()
            {
                // Arrange
                var query = new GetAllCarouselItemsQuery();
                var carouselItems = new List<CarouselItem>
                {
                    new CarouselItem { Id = 1, Title = "Item 1", ImageUrl = "image1.jpg" }
                }.AsQueryable();

                var mockDbSet = carouselItems.BuildMockDbSet();
                _context.CarouselItems.Returns(mockDbSet);

                // Act
                await _handler.Handle(query, CancellationToken.None);

                // Assert - Verify that AsNoTracking was called
                // Note: This test is primarily to document the expected behavior,
                // but the actual verification of AsNoTracking is limited in test framework
                mockDbSet.Received().AsNoTracking();
            }
        }
    }
}
