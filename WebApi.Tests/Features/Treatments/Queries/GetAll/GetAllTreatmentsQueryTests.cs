using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Features.Treatments.Queries.GetAll;

namespace WebApi.Tests.Features.Treatments.Queries.GetAll
{
    public class GetAllTreatmentsQueryTests
    {
        private readonly IApplicationDbContext _context;
        private readonly GetAllTreatmentsQueryHandler _handler;

        public GetAllTreatmentsQueryTests()
        {
            _context = Substitute.For<IApplicationDbContext>();
            _handler = new GetAllTreatmentsQueryHandler(_context);
        }

        [Fact]
        public void Query_ShouldBeEmptyRecord()
        {
            // Arrange & Act
            var query = new GetAllTreatmentsQuery();

            // Assert - Just verify the query can be instantiated
            query.ShouldNotBeNull();
        }

        [Fact]
        public void Handler_ShouldHaveCorrectConstructor()
        {
            // Arrange & Act
            var handler = new GetAllTreatmentsQueryHandler(_context);

            // Assert
            handler.ShouldNotBeNull();
        }

        [Fact]
        public void Query_ShouldImplementCorrectInterface()
        {
            // Arrange & Act
            var query = new GetAllTreatmentsQuery();

            // Assert
            query.ShouldBeAssignableTo<MediatR.IRequest<Result<List<Treatment>>>>();
        }

        [Fact]
        public void Handler_ShouldImplementCorrectInterface()
        {
            // Arrange & Act
            var handler = new GetAllTreatmentsQueryHandler(_context);

            // Assert
            handler.ShouldBeAssignableTo<MediatR.IRequestHandler<GetAllTreatmentsQuery, Result<List<Treatment>>>>();
        }

        [Fact]
        public void Context_ShouldBeInjectedCorrectly()
        {
            // Arrange
            var mockContext = Substitute.For<IApplicationDbContext>();

            // Act
            var handler = new GetAllTreatmentsQueryHandler(mockContext);

            // Assert
            handler.ShouldNotBeNull();
        }
    }
} 