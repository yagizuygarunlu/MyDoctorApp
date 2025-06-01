using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Domain.Enums;
using WebApi.DTOs;
using WebApi.Features.Appointments.Queries.GetAppointments;

namespace WebApi.Tests.Features.Appointments.Queries.GetAppointments
{
    public class GetAppointmentsQueryTests
    {
        private readonly IApplicationDbContext _context;
        private readonly GetAppointmentsQueryHandler _handler;

        public GetAppointmentsQueryTests()
        {
            _context = Substitute.For<IApplicationDbContext>();
            _handler = new GetAppointmentsQueryHandler(_context);
        }

        [Fact]
        public void Query_ShouldContainCorrectProperties()
        {
            // Arrange & Act
            var query = new GetAppointmentsQuery(2, DateTime.Now, DateTime.Now.AddDays(1), "Patient", AppointmentStatus.Pending);

            // Assert
            query.DoctorId.ShouldBe(2);
            query.PatientName.ShouldBe("Patient");
            query.AppointmentStatus.ShouldBe(AppointmentStatus.Pending);
        }

        [Fact]
        public void Query_ShouldHaveParameterlessConstructor()
        {
            // Arrange & Act
            var query = new GetAppointmentsQuery();

            // Assert
            query.ShouldNotBeNull();
            query.DoctorId.ShouldBeNull();
            query.PatientName.ShouldBeNull();
            query.AppointmentStatus.ShouldBeNull();
        }

        [Fact]
        public void Handler_ShouldHaveCorrectConstructor()
        {
            // Arrange & Act
            var handler = new GetAppointmentsQueryHandler(_context);

            // Assert
            handler.ShouldNotBeNull();
        }

        [Fact]
        public void Query_ShouldImplementCorrectInterface()
        {
            // Arrange & Act
            var query = new GetAppointmentsQuery();

            // Assert
            query.ShouldBeAssignableTo<MediatR.IRequest<Result<List<AppointmentDto>>>>();
        }

        [Fact]
        public void Handler_ShouldImplementCorrectInterface()
        {
            // Arrange & Act
            var handler = new GetAppointmentsQueryHandler(_context);

            // Assert
            handler.ShouldBeAssignableTo<MediatR.IRequestHandler<GetAppointmentsQuery, Result<List<AppointmentDto>>>>();
        }

        [Fact]
        public void Context_ShouldBeInjectedCorrectly()
        {
            // Arrange
            var mockContext = Substitute.For<IApplicationDbContext>();

            // Act
            var handler = new GetAppointmentsQueryHandler(mockContext);

            // Assert
            handler.ShouldNotBeNull();
        }

        [Fact]
        public void Query_Parse_ShouldReturnValidQuery()
        {
            // Arrange
            var testString = "test";

            // Act
            var result = GetAppointmentsQuery.Parse(testString, null);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<GetAppointmentsQuery>();
        }

        [Fact]
        public void Query_TryParse_ShouldReturnTrue()
        {
            // Arrange
            var testString = "test";

            // Act
            var success = GetAppointmentsQuery.TryParse(testString, null, out var result);

            // Assert
            success.ShouldBeTrue();
            result.ShouldNotBeNull();
        }
    }
} 