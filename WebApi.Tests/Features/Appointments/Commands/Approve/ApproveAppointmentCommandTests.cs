using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Domain.Enums;
using WebApi.Features.Appointments.Commands.Approve;

namespace WebApi.Tests.Features.Appointments.Commands.Approve
{
    public class ApproveAppointmentCommandTests
    {
        private readonly IApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;
        private readonly ApproveAppointmentCommandHandler _handler;
        private readonly Guid _testAppointmentId = Guid.NewGuid();
        private readonly string _testDoctorId = "doctor1";
        private readonly string _notFoundMessage = "Appointment not found";

        public ApproveAppointmentCommandTests()
        {
            _context = Substitute.For<IApplicationDbContext>();
            _localizationService = Substitute.For<ILocalizationService>();
            _handler = new ApproveAppointmentCommandHandler(_context, _localizationService);
            _localizationService.GetLocalizedString(LocalizationKeys.Appointments.NotFound)
                .Returns(_notFoundMessage);
        }

        [Fact]
        public async Task Handle_WhenAppointmentExists_ShouldApproveAppointmentAndReturnSuccess()
        {
            // Arrange
            var appointment = new Appointment
            {
                Id = _testAppointmentId,
                Status = AppointmentStatus.Pending
            };

            var dbSetMock = Substitute.For<DbSet<Appointment>>();
            _context.Appointments.Returns(dbSetMock);

            _context.Appointments
                .FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
                .Returns(appointment);

            var command = new ApproveAppointmentCommand(_testAppointmentId, _testDoctorId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();
            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldBe(Unit.Value);

            appointment.Status.ShouldBe(AppointmentStatus.Approved);
            await _context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenAppointmentDoesNotExist_ShouldReturnFailureResult()
        {
            // Arrange
            _context.Appointments
                .FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
                .Returns(callInfo => ValueTask.FromResult<Appointment?>(null)); 

            var command = new ApproveAppointmentCommand(_testAppointmentId, _testDoctorId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeFalse();
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(_notFoundMessage);

            await _context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Command_ShouldContainCorrectProperties()
        {
            // Arrange & Act
            var command = new ApproveAppointmentCommand(_testAppointmentId, _testDoctorId);

            // Assert
            command.AppointmentId.ShouldBe(_testAppointmentId);
            command.DoctorId.ShouldBe(_testDoctorId);
        }

        [Fact]
        public void Handler_ShouldImplementIRequestHandlerInterface()
        {
            // Assert
            _handler.ShouldBeAssignableTo<IRequestHandler<ApproveAppointmentCommand, Result<Unit>>>();
        }
    }
}
