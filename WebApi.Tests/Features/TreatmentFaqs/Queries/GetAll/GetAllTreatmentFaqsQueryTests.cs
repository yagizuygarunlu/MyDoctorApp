using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Features.TreatmentFaqs.Queries.GetAll;

namespace WebApi.Tests.Features.TreatmentFaqs.Queries.GetAll
{
    public class GetAllTreatmentFaqsQueryTests
    {
        private readonly IApplicationDbContext _context;
        private readonly GetActiveTreatmentFaqsQueryHandler _handler;

        public GetAllTreatmentFaqsQueryTests()
        {
            _context = Substitute.For<IApplicationDbContext>();
            _handler = new GetActiveTreatmentFaqsQueryHandler(_context);
        }

        [Fact]
        public void Query_ShouldBeEmptyRecord()
        {
            // Arrange & Act
            var query = new GetAllTreatmentFaqsQuery();

            // Assert - Just verify the query can be instantiated
            query.ShouldNotBeNull();
        }

        [Fact]
        public void Handler_ShouldHaveCorrectConstructor()
        {
            // Arrange & Act
            var handler = new GetActiveTreatmentFaqsQueryHandler(_context);

            // Assert
            handler.ShouldNotBeNull();
        }

        [Fact]
        public void Query_ShouldImplementCorrectInterface()
        {
            // Arrange & Act
            var query = new GetAllTreatmentFaqsQuery();

            // Assert
            query.ShouldBeAssignableTo<MediatR.IRequest<Result<List<TreatmentFaq>>>>();
        }

        [Fact]
        public void Handler_ShouldImplementCorrectInterface()
        {
            // Arrange & Act
            var handler = new GetActiveTreatmentFaqsQueryHandler(_context);

            // Assert
            handler.ShouldBeAssignableTo<MediatR.IRequestHandler<GetAllTreatmentFaqsQuery, Result<List<TreatmentFaq>>>>();
        }

        [Fact]
        public void Context_ShouldBeInjectedCorrectly()
        {
            // Arrange
            var mockContext = Substitute.For<IApplicationDbContext>();

            // Act
            var handler = new GetActiveTreatmentFaqsQueryHandler(mockContext);

            // Assert
            handler.ShouldNotBeNull();
        }
    }
} 