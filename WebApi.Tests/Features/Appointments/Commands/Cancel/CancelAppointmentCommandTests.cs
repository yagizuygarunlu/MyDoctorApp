using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Domain.Enums;
using WebApi.Features.Appointments.Commands.Cancel;

namespace WebApi.Tests.Features.Appointments.Commands.Cancel
{
    public class CancelAppointmentCommandTests
    {
        private readonly IApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;
        private readonly CancelAppointmentCommandHandler _handler;
        private readonly Guid _testAppointmentId = Guid.NewGuid();
        private readonly string _cancelledMessage = "Appointment cancelled";

        public CancelAppointmentCommandTests()
        {
            _context = Substitute.For<IApplicationDbContext>();
            _localizationService = Substitute.For<ILocalizationService>();
            _handler = new CancelAppointmentCommandHandler(_context, _localizationService);
            _localizationService.GetLocalizedString(LocalizationKeys.Appointments.Cancelled)
                .Returns(_cancelledMessage);
        }

        [Fact]
        public async Task Handle_WhenAppointmentExists_ShouldCancelAppointmentAndReturnSuccess()
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
                .FindAsync(Arg.Is(_testAppointmentId))
                .Returns(appointment);

            var command = new CancelAppointmentCommand(_testAppointmentId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();
            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldBe(Unit.Value);

            appointment.Status.ShouldBe(AppointmentStatus.Cancelled);
            await _context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenAppointmentDoesNotExist_ShouldReturnFailureResult()
        {
            // Arrange
            _context.Appointments
                .FindAsync(Arg.Any<object>())
                .Returns((Appointment?)null);

            var command = new CancelAppointmentCommand(_testAppointmentId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeFalse();
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(_cancelledMessage);

            await _context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Command_ShouldContainCorrectProperties()
        {
            // Arrange & Act
            var command = new CancelAppointmentCommand(_testAppointmentId);

            // Assert
            command.Id.ShouldBe(_testAppointmentId);
        }

        [Fact]
        public void Handler_ShouldImplementIRequestHandlerInterface()
        {
            // Assert
            _handler.ShouldBeAssignableTo<IRequestHandler<CancelAppointmentCommand, Result<Unit>>>();
        }
    }
}
