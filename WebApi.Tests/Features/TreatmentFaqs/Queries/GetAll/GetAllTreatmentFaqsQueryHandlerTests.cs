using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Features.TreatmentFaqs.Queries.GetAll;
using Xunit;

namespace WebApi.Tests.Features.TreatmentFaqs.Queries.GetAll
{
    public class GetAllTreatmentFaqsQueryHandlerTests
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly GetActiveTreatmentFaqsQueryHandler _handler;

        public GetAllTreatmentFaqsQueryHandlerTests()
        {
            _dbContext = Substitute.For<IApplicationDbContext>();
            _handler = new GetActiveTreatmentFaqsQueryHandler(_dbContext);
        }

        [Fact]
        public async Task Handle_ShouldReturnAllTreatmentFaqs()
        {
            // Arrange
            var treatmentFaqs = new List<TreatmentFaq>
            {
                new TreatmentFaq { Id = 1, TreatmentId = 1, Question = "Q1", Answer = "A1", Treatment = new Treatment(){
                Name=""}
                },
                new TreatmentFaq { Id = 2, TreatmentId = 2, Question = "Q2", Answer = "A2", Treatment = new Treatment{
                Name=""
                } }
            }.AsQueryable();

            var dbSet = treatmentFaqs.BuildMockDbSet();

            _dbContext.TreatmentFaqs.Returns(dbSet);

            // Act
            var result = await _handler.Handle(new GetAllTreatmentFaqsQuery(), CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.Count.ShouldBe(2);
            result.Data[0].Question.ShouldBe("Q1");
            result.Data[1].Question.ShouldBe("Q2");
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoTreatmentFaqsExist()
        {
            // Arrange
            var treatmentFaqs = new List<TreatmentFaq>().AsQueryable();

            var dbSet = treatmentFaqs.BuildMockDbSet();

            _dbContext.TreatmentFaqs.Returns(dbSet);

            // Act
            var result = await _handler.Handle(new GetAllTreatmentFaqsQuery(), CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.ShouldBeEmpty();
        }
    }
}