using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Domain.Entities;
using WebApi.Features.Appointments.Commands.Update;

namespace WebApi.Tests.Features.Appointments.Commands.Update
{
    public class UpdateAppointmentCommandTests
    {
        #region Validator Tests

        public class UpdateAppointmentCommandValidatorTests
        {
            private readonly ILocalizationService _localizationService;
            private readonly UpdateAppointmentCommandValidator _validator;

            public UpdateAppointmentCommandValidatorTests()
            {
                // Arrange
                _localizationService = Substitute.For<ILocalizationService>();
                SetupLocalizationStrings();
                _validator = new UpdateAppointmentCommandValidator(_localizationService);
            }

            private void SetupLocalizationStrings()
            {
                _localizationService.GetLocalizedString(LocalizationKeys.Appointments.DoctorRequired)
                    .Returns("Doctor is required");
                _localizationService.GetLocalizedString(LocalizationKeys.Appointments.DoctorIdMustBeGreaterThanZero)
                    .Returns("Doctor ID must be greater than zero");
                _localizationService.GetLocalizedString(LocalizationKeys.Patients.NameRequired)
                    .Returns("Patient name is required");
                _localizationService.GetLocalizedString(LocalizationKeys.Patients.EmailRequired)
                    .Returns("Patient email is required");
                _localizationService.GetLocalizedString(LocalizationKeys.Patients.InvalidEmail)
                    .Returns("Invalid email format");
                _localizationService.GetLocalizedString(LocalizationKeys.Patients.PhoneRequired)
                    .Returns("Patient phone is required");
                _localizationService.GetLocalizedString(LocalizationKeys.Patients.InvalidPhone)
                    .Returns("Invalid phone format");
                _localizationService.GetLocalizedString(LocalizationKeys.Appointments.DateRequired)
                    .Returns("Appointment date is required");
                _localizationService.GetLocalizedString(LocalizationKeys.Appointments.DateMustBeInFuture)
                    .Returns("Appointment date must be in the future");
            }

            [Fact]
            public void Validator_ShouldPass_ForValidCommand()
            {
                // Arrange
                var command = new UpdateAppointmentCommand(
                    Id: Guid.NewGuid(),
                    DoctorId: 5,
                    PatientName: "John Doe",
                    PatientEmail: "john.doe@example.com",
                    PatientPhone: "+12345678901",
                    AppointmentDate: DateTime.UtcNow.AddDays(7),
                    Notes: "Regular checkup");

                // Act
                var result = _validator.TestValidate(command);

                // Assert
                result.ShouldNotHaveAnyValidationErrors();
            }

            [Theory]
            [InlineData(0, "Doctor ID must be greater than zero")]
            [InlineData(-1, "Doctor ID must be greater than zero")]
            public void Validator_ShouldFail_WhenDoctorIdIsInvalid(int doctorId, string expectedError)
            {
                // Arrange
                var command = new UpdateAppointmentCommand(
                    Id: Guid.NewGuid(),
                    DoctorId: doctorId,
                    PatientName: "John Doe",
                    PatientEmail: "john.doe@example.com",
                    PatientPhone: "+12345678901",
                    AppointmentDate: DateTime.UtcNow.AddDays(7),
                    Notes: "Regular checkup");

                // Act
                var result = _validator.TestValidate(command);

                // Assert
                result.ShouldHaveValidationErrorFor(x => x.DoctorId)
                    .WithErrorMessage(expectedError);
            }

            [Theory]
            [InlineData("", "Patient name is required")]
            [InlineData(null, "Patient name is required")]
            public void Validator_ShouldFail_WhenPatientNameIsInvalid(string name, string expectedError)
            {
                // Arrange
                var command = new UpdateAppointmentCommand(
                    Id: Guid.NewGuid(),
                    DoctorId: 5,
                    PatientName: name,
                    PatientEmail: "john.doe@example.com",
                    PatientPhone: "+12345678901",
                    AppointmentDate: DateTime.UtcNow.AddDays(7),
                    Notes: "Regular checkup");

                // Act
                var result = _validator.TestValidate(command);

                // Assert
                result.ShouldHaveValidationErrorFor(x => x.PatientName)
                    .WithErrorMessage(expectedError);
            }

            [Theory]
            [InlineData("", "Patient email is required")]
            [InlineData(null, "Patient email is required")]
            [InlineData("not-an-email", "Invalid email format")]
            [InlineData("missing@domain", "Invalid email format")]
            public void Validator_ShouldFail_WhenPatientEmailIsInvalid(string email, string expectedError)
            {
                // Arrange
                var command = new UpdateAppointmentCommand(
                    Id: Guid.NewGuid(),
                    DoctorId: 5,
                    PatientName: "John Doe",
                    PatientEmail: email,
                    PatientPhone: "+12345678901",
                    AppointmentDate: DateTime.UtcNow.AddDays(7),
                    Notes: "Regular checkup");

                // Act
                var result = _validator.TestValidate(command);

                // Assert
                result.ShouldHaveValidationErrorFor(x => x.PatientEmail)
                    .WithErrorMessage(expectedError);
            }

            [Theory]
            [InlineData("", "Patient phone is required")]
            [InlineData(null, "Patient phone is required")]
            [InlineData("abc", "Invalid phone format")]
            [InlineData("123", "Invalid phone format")]
            public void Validator_ShouldFail_WhenPatientPhoneIsInvalid(string phone, string expectedError)
            {
                // Arrange
                var command = new UpdateAppointmentCommand(
                    Id: Guid.NewGuid(),
                    DoctorId: 5,
                    PatientName: "John Doe",
                    PatientEmail: "john.doe@example.com",
                    PatientPhone: phone,
                    AppointmentDate: DateTime.UtcNow.AddDays(7),
                    Notes: "Regular checkup");

                // Act
                var result = _validator.TestValidate(command);

                // Assert
                result.ShouldHaveValidationErrorFor(x => x.PatientPhone)
                    .WithErrorMessage(expectedError);
            }

            [Fact]
            public void Validator_ShouldFail_WhenAppointmentDateIsInPast()
            {
                // Arrange
                var command = new UpdateAppointmentCommand(
                    Id: Guid.NewGuid(),
                    DoctorId: 5,
                    PatientName: "John Doe",
                    PatientEmail: "john.doe@example.com",
                    PatientPhone: "+12345678901",
                    AppointmentDate: DateTime.UtcNow.AddDays(-1),
                    Notes: "Regular checkup");

                // Act
                var result = _validator.TestValidate(command);

                // Assert
                result.ShouldHaveValidationErrorFor(x => x.AppointmentDate)
                    .WithErrorMessage("Appointment date must be in the future");
            }
        }

        #endregion

        #region Handler Tests

        public class UpdateAppointmentCommandHandlerTests
        {
            private readonly IApplicationDbContext _context;
            private readonly ILocalizationService _localizationService;
            private readonly UpdateAppointmentCommandHandler _handler;

            public UpdateAppointmentCommandHandlerTests()
            {
                // Arrange
                _context = Substitute.For<IApplicationDbContext>();
                _localizationService = Substitute.For<ILocalizationService>();
                _handler = new UpdateAppointmentCommandHandler(_context, _localizationService);
            }

            [Fact]
            public async Task Handle_WhenAppointmentExists_ShouldUpdateAppointmentAndPatientDetails()
            {
                // Arrange
                var appointmentId = Guid.NewGuid();
                var patientId = Guid.NewGuid();
                var command = new UpdateAppointmentCommand(
                    Id: appointmentId,
                    DoctorId: 5,
                    PatientName: "John Doe Updated",
                    PatientEmail: "john.updated@example.com",
                    PatientPhone: "+12345678901",
                    AppointmentDate: DateTime.UtcNow.AddDays(10),
                    Notes: "Follow-up appointment");

                var appointment = new Appointment { Id = appointmentId, PatientId = patientId };
                var patient = new Patient
                {
                    Id = patientId,
                    FullName = "John Doe",
                    Email = "john.doe@example.com",
                    PhoneNumber = "+10987654321"
                };

                _context.Appointments.FindAsync(appointmentId).Returns(appointment);
                _context.Patients.FindAsync(patientId).Returns(patient);

                // Act
                var result = await _handler.Handle(command, CancellationToken.None);

                // Assert
                result.Succeeded.ShouldBeTrue();
                result.IsSuccess.ShouldBeTrue();

                // Verify appointment was updated correctly
                appointment.DoctorId.ShouldBe(command.DoctorId);
                appointment.Date.ShouldBe(command.AppointmentDate);
                appointment.Description.ShouldBe(command.Notes);

                // Verify patient was updated correctly
                patient.FullName.ShouldBe(command.PatientName);
                patient.Email.ShouldBe(command.PatientEmail);
                patient.PhoneNumber.ShouldBe(command.PatientPhone);

                // Verify save was called
                await _context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
            }

            [Fact]
            public async Task Handle_WhenAppointmentDoesNotExist_ShouldReturnFailureResult()
            {
                // Arrange
                var appointmentId = Guid.NewGuid(); // Non-existent ID
                var command = new UpdateAppointmentCommand(
                    Id: appointmentId,
                    DoctorId: 5,
                    PatientName: "John Doe",
                    PatientEmail: "john.doe@example.com",
                    PatientPhone: "+12345678901",
                    AppointmentDate: DateTime.UtcNow.AddDays(7),
                    Notes: "Regular checkup");

                var localizedErrorMessage = "Appointment not found";

                _context.Appointments.FindAsync(appointmentId).Returns((Appointment)null);
                _localizationService.GetLocalizedString(LocalizationKeys.Appointments.NotFound)
                    .Returns(localizedErrorMessage);

                // Act
                var result = await _handler.Handle(command, CancellationToken.None);

                // Assert
                result.Succeeded.ShouldBeFalse();
                result.IsFailure.ShouldBeTrue();
                result.Error.ShouldBe(localizedErrorMessage);

                // Verify save was not called
                await _context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
            }

            [Fact]
            public async Task Handle_WhenExceptionOccurs_ShouldPropagateException()
            {
                // Arrange
                var appointmentId = Guid.NewGuid();
                var patientId = Guid.NewGuid();
                var command = new UpdateAppointmentCommand(
                    Id: appointmentId,
                    DoctorId: 5,
                    PatientName: "John Doe",
                    PatientEmail: "john.doe@example.com",
                    PatientPhone: "+12345678901",
                    AppointmentDate: DateTime.UtcNow.AddDays(7),
                    Notes: "Regular checkup");

                var appointment = new Appointment { Id = appointmentId, PatientId = patientId };
                var patient = new Patient { Id = patientId, Email = "", FullName = "", PhoneNumber = "" };

                _context.Appointments.FindAsync(appointmentId).Returns(appointment);
                _context.Patients.FindAsync(patientId).Returns(patient);
                _context.SaveChangesAsync(Arg.Any<CancellationToken>())
                    .ThrowsAsync(new DbUpdateException("Database error occurred"));

                // Act & Assert
                await Should.ThrowAsync<DbUpdateException>(async () =>
                    await _handler.Handle(command, CancellationToken.None)
                );
            }
        }

        #endregion
    }
}
