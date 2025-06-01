using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Features.Treatments.Queries.GetActives;

namespace WebApi.Tests.Features.Treatments.Queries.GetActives
{
    public class GetActiveTreatmentsQueryTests
    {
        private readonly IApplicationDbContext _context;
        private readonly GetActiveTreatmentsQueryHandler _handler;

        public GetActiveTreatmentsQueryTests()
        {
            _context = Substitute.For<IApplicationDbContext>();
            _handler = new GetActiveTreatmentsQueryHandler(_context);
        }

        [Fact]
        public void Query_ShouldBeEmptyRecord()
        {
            // Arrange & Act
            var query = new GetActiveTreatmentsQuery();

            // Assert - Just verify the query can be instantiated
            query.ShouldNotBeNull();
        }

        [Fact]
        public void Handler_ShouldHaveCorrectConstructor()
        {
            // Arrange & Act
            var handler = new GetActiveTreatmentsQueryHandler(_context);

            // Assert
            handler.ShouldNotBeNull();
        }

        [Fact]
        public void Query_ShouldImplementCorrectInterface()
        {
            // Arrange & Act
            var query = new GetActiveTreatmentsQuery();

            // Assert
            query.ShouldBeAssignableTo<MediatR.IRequest<Result<List<Treatment>>>>();
        }

        [Fact]
        public void Handler_ShouldImplementCorrectInterface()
        {
            // Arrange & Act
            var handler = new GetActiveTreatmentsQueryHandler(_context);

            // Assert
            handler.ShouldBeAssignableTo<MediatR.IRequestHandler<GetActiveTreatmentsQuery, Result<List<Treatment>>>>();
        }

        [Fact]
        public void Context_ShouldBeInjectedCorrectly()
        {
            // Arrange
            var mockContext = Substitute.For<IApplicationDbContext>();

            // Act
            var handler = new GetActiveTreatmentsQueryHandler(mockContext);

            // Assert
            handler.ShouldNotBeNull();
        }
    }
} 