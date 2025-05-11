using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Domain.Enums;
using WebApi.Features.Appointments.Commands.Reject;

namespace WebApi.Tests.Features.Appointments.Commands.Reject
{
    public class RejectAppointmentCommandTests
    {
        private readonly IApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;
        private readonly RejectAppointmentCommandHandler _handler;

        public RejectAppointmentCommandTests()
        {
            // Arrange
            _context = Substitute.For<IApplicationDbContext>();
            _localizationService = Substitute.For<ILocalizationService>();
            _handler = new RejectAppointmentCommandHandler(_context, _localizationService);
        }

        [Fact]
        public async Task Handle_WhenAppointmentExists_ShouldRejectAppointmentAndReturnSuccess()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var rejectionReason = "Schedule conflict";
            var appointment = new WebApi.Domain.Entities.Appointment { Id = appointmentId };

            _context.Appointments.FindAsync(appointmentId).Returns(appointment);

            // Act
            var result = await _handler.Handle(
                new RejectAppointmentCommand(appointmentId, rejectionReason),
                CancellationToken.None);

            // Assert
            result.Succeeded.ShouldBeTrue();
            result.IsSuccess.ShouldBeTrue();
            appointment.Status.ShouldBe(AppointmentStatus.Rejected);
            appointment.RejectionReason.ShouldBe(rejectionReason);

            await _context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenAppointmentDoesNotExist_ShouldReturnFailureResult()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var rejectionReason = "Schedule conflict";
            var localizedErrorMessage = "Appointment rejection failed";

            _context.Appointments.FindAsync(appointmentId).Returns((WebApi.Domain.Entities.Appointment)null);
            _localizationService.GetLocalizedString(LocalizationKeys.Appointments.Rejected)
                .Returns(localizedErrorMessage);

            // Act
            var result = await _handler.Handle(
                new RejectAppointmentCommand(appointmentId, rejectionReason),
                CancellationToken.None);

            // Assert
            result.Succeeded.ShouldBeFalse();
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(localizedErrorMessage);

            await _context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Theory]
        [InlineData("")]
        [InlineData("Patient requested cancellation")]
        [InlineData("Doctor unavailable due to emergency")]
        public async Task Handle_WithDifferentRejectionReasons_ShouldSetCorrectReason(string rejectionReason)
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var appointment = new WebApi.Domain.Entities.Appointment { Id = appointmentId };

            _context.Appointments.FindAsync(appointmentId).Returns(appointment);

            // Act
            var result = await _handler.Handle(
                new RejectAppointmentCommand(appointmentId, rejectionReason),
                CancellationToken.None);

            // Assert
            result.Succeeded.ShouldBeTrue();
            appointment.RejectionReason.ShouldBe(rejectionReason);
        }

        [Fact]
        public async Task Handle_WhenExceptionOccurs_ShouldPropagateException()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var rejectionReason = "Schedule conflict";

            _context.Appointments.FindAsync(appointmentId)
                .Returns(new WebApi.Domain.Entities.Appointment { Id = appointmentId });
            _context.SaveChangesAsync(Arg.Any<CancellationToken>())
                .ThrowsAsync(new DbUpdateException("Database error occurred"));

            // Act & Assert
            await Should.ThrowAsync<DbUpdateException>(async () =>
                await _handler.Handle(
                    new RejectAppointmentCommand(appointmentId, rejectionReason),
                    CancellationToken.None)
            );
        }
    }
}
