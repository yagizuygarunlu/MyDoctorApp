using FluentValidation.TestHelper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Domain.Enums;
using WebApi.Features.Appointments.Commands.Create;
using WebApi.Features.Patients.Commands.Create;

namespace WebApi.Tests.Features.Appointments.Commands.Create
{
    public class CreateAppointmentCommandTests
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;
        private readonly ILocalizationService _localizationService;
        private readonly CreateAppointmentCommandHandler _handler;
        private readonly CreateAppointmentCommandValidator _validator;

        // Test data
        private readonly CreateAppointmentCommand _validCommand;
        private readonly int _validDoctorId = 1;
        private readonly string _validPatientName = "John Doe";
        private readonly string _validPatientEmail = "john@example.com";
        private readonly string _validPatientPhone = "+12345678901";
        private readonly DateTime _validFutureDate;
        private readonly string _validNotes = "Test appointment";
        private readonly Guid _patientId = Guid.NewGuid();

        // Localization messages
        private readonly string _doctorRequiredMessage = "Doctor is required";
        private readonly string _doctorIdInvalidMessage = "Doctor ID must be greater than zero";
        private readonly string _nameRequiredMessage = "Name is required";
        private readonly string _emailRequiredMessage = "Email is required";
        private readonly string _invalidEmailMessage = "Invalid email format";
        private readonly string _phoneRequiredMessage = "Phone is required";
        private readonly string _invalidPhoneMessage = "Invalid phone format";
        private readonly string _dateRequiredMessage = "Date is required";
        private readonly string _dateFutureMessage = "Date must be in the future";
        private readonly string _doctorUnavailableMessage = "Doctor is unavailable at this time";
        private readonly string _patientUnavailableMessage = "Patient already has an appointment on this date";
        private readonly string _patientCreationFailedMessage = "Failed to create patient";

        public CreateAppointmentCommandTests()
        {
            // Initialize test data
            _validFutureDate = DateTime.UtcNow.AddDays(1);

            _validCommand = new CreateAppointmentCommand(
                _validDoctorId,
                _validPatientName,
                _validPatientEmail,
                _validPatientPhone,
                _validFutureDate,
                _validNotes
            );

            // Set up mocks
            _context = Substitute.For<IApplicationDbContext>();
            _mediator = Substitute.For<IMediator>();
            _localizationService = Substitute.For<ILocalizationService>();

            // Configure localization service
            SetupLocalizationService();

            // Initialize handler and validator
            _handler = new CreateAppointmentCommandHandler(_context, _mediator, _localizationService);
            _validator = new CreateAppointmentCommandValidator(_context, _localizationService);
        }

        private void SetupLocalizationService()
        {
            _localizationService.GetLocalizedString(LocalizationKeys.Appointments.DoctorRequired)
                .Returns(_doctorRequiredMessage);
            _localizationService.GetLocalizedString(LocalizationKeys.Appointments.DoctorIdMustBeGreaterThanZero)
                .Returns(_doctorIdInvalidMessage);
            _localizationService.GetLocalizedString(LocalizationKeys.Patients.NameRequired)
                .Returns(_nameRequiredMessage);
            _localizationService.GetLocalizedString(LocalizationKeys.Patients.EmailRequired)
                .Returns(_emailRequiredMessage);
            _localizationService.GetLocalizedString(LocalizationKeys.Patients.InvalidEmail)
                .Returns(_invalidEmailMessage);
            _localizationService.GetLocalizedString(LocalizationKeys.Patients.PhoneRequired)
                .Returns(_phoneRequiredMessage);
            _localizationService.GetLocalizedString(LocalizationKeys.Patients.InvalidPhone)
                .Returns(_invalidPhoneMessage);
            _localizationService.GetLocalizedString(LocalizationKeys.Appointments.DateRequired)
                .Returns(_dateRequiredMessage);
            _localizationService.GetLocalizedString(LocalizationKeys.Appointments.DateMustBeInFuture)
                .Returns(_dateFutureMessage);
            _localizationService.GetLocalizedString(LocalizationKeys.Appointments.DoctorUnavailable)
                .Returns(_doctorUnavailableMessage);
            _localizationService.GetLocalizedString(LocalizationKeys.Appointments.PatientAlreadyHasAppointment)
                .Returns(_patientUnavailableMessage);
            _localizationService.GetLocalizedString(LocalizationKeys.Patients.CreatingFailed)
                .Returns(_patientCreationFailedMessage);
        }

        #region Command Handler Tests

        [Fact]
        public async Task Handle_WhenPatientCreationSucceeds_ShouldCreateAppointmentAndReturnSuccess()
        {
            // Arrange
            var patientResult = Result<Guid>.Success(_patientId);
            _mediator.Send(Arg.Any<CreatePatientCommand>(), Arg.Any<CancellationToken>())
                .Returns(patientResult);

            var appointmentsDbSet = Substitute.For<DbSet<Appointment>>();
            _context.Appointments.Returns(appointmentsDbSet);

            // Capture the added appointment
            Appointment capturedAppointment = null;
            appointmentsDbSet.When(x => x.Add(Arg.Any<Appointment>()))
                .Do(callInfo => capturedAppointment = callInfo.Arg<Appointment>());

            // Act
            var result = await _handler.Handle(_validCommand, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            // Verify the appointment was added with correct data
            appointmentsDbSet.Received(1).Add(Arg.Any<Appointment>());
            capturedAppointment.ShouldNotBeNull();
            capturedAppointment.DoctorId.ShouldBe(_validDoctorId);
            capturedAppointment.PatientId.ShouldBe(_patientId);
            capturedAppointment.Date.ShouldBe(_validFutureDate);
            capturedAppointment.Description.ShouldBe(_validNotes);
            capturedAppointment.Status.ShouldBe(AppointmentStatus.Pending); // Default status should be Pending

            await _context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

            // Verify patient creation was called with correct data
            await _mediator.Received(1).Send(
                Arg.Is<CreatePatientCommand>(cmd =>
                    cmd.Name == _validPatientName &&
                    cmd.Email == _validPatientEmail &&
                    cmd.Phone == _validPatientPhone),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenPatientCreationFails_ShouldReturnFailureResult()
        {
            // Arrange
            var patientResult = Result<Guid>.Failure("Patient creation failed");
            _mediator.Send(Arg.Any<CreatePatientCommand>(), Arg.Any<CancellationToken>())
                .Returns(patientResult);

            // Act
            var result = await _handler.Handle(_validCommand, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeFalse();
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(_patientCreationFailedMessage);

            // Verify no appointment was created
            _context.Appointments.DidNotReceive().Add(Arg.Any<Appointment>());
            await _context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        #endregion

        #region Validator Tests - Required Fields

        [Fact]
        public async Task Validate_WhenDoctorIdIsZero_ShouldHaveValidationError()
        {
            // Arrange
            var command = _validCommand with { DoctorId = 0 };

            // Create empty collections for database context
            var emptyAppointments = new List<Appointment>().AsQueryable().BuildMockDbSet();
            _context.Appointments.Returns(emptyAppointments);

            var emptyDoctors = new List<Doctor>().AsQueryable().BuildMockDbSet();
            _context.Doctors.Returns(emptyDoctors);

            var emptyPatients = new List<Patient>().AsQueryable().BuildMockDbSet();
            _context.Patients.Returns(emptyPatients);

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.DoctorId)
                .WithErrorMessage(_doctorIdInvalidMessage);
        }

        [Fact]
        public async Task Validate_WhenPatientNameIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var command = _validCommand with { PatientName = string.Empty };

            // Set up mocks for all database sets used in validator
            var emptyAppointments = new List<Appointment>().AsQueryable().BuildMockDbSet();
            _context.Appointments.Returns(emptyAppointments);

            var emptyDoctors = new List<Doctor>().AsQueryable().BuildMockDbSet();
            _context.Doctors.Returns(emptyDoctors);

            var emptyPatients = new List<Patient>().AsQueryable().BuildMockDbSet();
            _context.Patients.Returns(emptyPatients);

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PatientName)
                .WithErrorMessage(_nameRequiredMessage);
        }


        [Fact]
        public async Task Validate_WhenPatientEmailIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var command = _validCommand with { PatientEmail = string.Empty };

            // Set up mocks for database context used in validator
            var emptyAppointments = new List<Appointment>().AsQueryable().BuildMockDbSet();
            _context.Appointments.Returns(emptyAppointments);

            var emptyDoctors = new List<Doctor>().AsQueryable().BuildMockDbSet();
            _context.Doctors.Returns(emptyDoctors);

            var emptyPatients = new List<Patient>().AsQueryable().BuildMockDbSet();
            _context.Patients.Returns(emptyPatients);

            // Act & Assert
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.PatientEmail)
                .WithErrorMessage(_emailRequiredMessage);
        }

        [Fact]
        public async Task Validate_WhenPatientEmailIsInvalid_ShouldHaveValidationError()
        {
            // Arrange
            var command = _validCommand with { PatientEmail = "invalid-email" };

            // Set up mocks for database context used in validator
            var emptyAppointments = new List<Appointment>().AsQueryable().BuildMockDbSet();
            _context.Appointments.Returns(emptyAppointments);

            var emptyDoctors = new List<Doctor>().AsQueryable().BuildMockDbSet();
            _context.Doctors.Returns(emptyDoctors);

            var emptyPatients = new List<Patient>().AsQueryable().BuildMockDbSet();
            _context.Patients.Returns(emptyPatients);

            // Act & Assert
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.PatientEmail)
                .WithErrorMessage(_invalidEmailMessage);
        }

        [Fact]
        public async Task Validate_WhenPatientPhoneIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var command = _validCommand with { PatientPhone = string.Empty };

            // Set up mocks for database context used in validator
            var emptyAppointments = new List<Appointment>().AsQueryable().BuildMockDbSet();
            _context.Appointments.Returns(emptyAppointments);

            var emptyDoctors = new List<Doctor>().AsQueryable().BuildMockDbSet();
            _context.Doctors.Returns(emptyDoctors);

            var emptyPatients = new List<Patient>().AsQueryable().BuildMockDbSet();
            _context.Patients.Returns(emptyPatients);

            // Act & Assert
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.PatientPhone)
                .WithErrorMessage(_phoneRequiredMessage);
        }

        [Fact]
        public async Task Validate_WhenPatientPhoneIsInvalid_ShouldHaveValidationError()
        {
            // Arrange
            var command = _validCommand with { PatientPhone = "invalid-phone" };

            // Set up mocks for database context used in validator
            var emptyAppointments = new List<Appointment>().AsQueryable().BuildMockDbSet();
            _context.Appointments.Returns(emptyAppointments);

            var emptyDoctors = new List<Doctor>().AsQueryable().BuildMockDbSet();
            _context.Doctors.Returns(emptyDoctors);

            var emptyPatients = new List<Patient>().AsQueryable().BuildMockDbSet();
            _context.Patients.Returns(emptyPatients);

            // Act & Assert
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.PatientPhone)
                .WithErrorMessage(_invalidPhoneMessage);
        }

        [Fact]
        public async Task Validate_WhenAppointmentDateIsInThePast_ShouldHaveValidationError()
        {
            // Arrange
            var command = _validCommand with { AppointmentDate = DateTime.UtcNow.AddDays(-1) };

            // Set up mocks for database context used in validator
            var emptyAppointments = new List<Appointment>().AsQueryable().BuildMockDbSet();
            _context.Appointments.Returns(emptyAppointments);

            var emptyDoctors = new List<Doctor>().AsQueryable().BuildMockDbSet();
            _context.Doctors.Returns(emptyDoctors);

            var emptyPatients = new List<Patient>().AsQueryable().BuildMockDbSet();
            _context.Patients.Returns(emptyPatients);

            // Act & Assert
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.AppointmentDate)
                .WithErrorMessage(_dateFutureMessage);
        }


        #endregion

        #region Validator Tests - Doctor and Patient Availability

        [Fact]
        public async Task Validate_WhenDoctorHasOverlappingAppointment_ShouldHaveValidationError()
        {
            // Arrange
            var appointmentDate = _validFutureDate;

            // Create the existing appointment
            var existingAppointment = new Appointment
            {
                Id = Guid.NewGuid(),
                DoctorId = _validDoctorId,
                Date = new DateTimeOffset(appointmentDate),
                Status = AppointmentStatus.Pending
            };

            // Create mock appointments DbSet with proper async support
            var existingAppointments = new List<Appointment> { existingAppointment }.AsQueryable().BuildMockDbSet();
            _context.Appointments.Returns(existingAppointments);

            // Create empty patients DbSet
            var emptyPatients = new List<Patient>().AsQueryable().BuildMockDbSet();
            _context.Patients.Returns(emptyPatients);

            // Create empty doctors DbSet
            var emptyDoctors = new List<Doctor>().AsQueryable().BuildMockDbSet();
            _context.Doctors.Returns(emptyDoctors);

            // Act & Assert
            var result = await _validator.TestValidateAsync(_validCommand);
            result.ShouldHaveValidationErrorFor(x => x)
                .WithErrorMessage(_doctorUnavailableMessage);
        }

        [Fact]
        public async Task Validate_WhenPatientHasAppointmentOnSameDay_ShouldHaveValidationError()
        {
            // Arrange
            var existingPatient = new Patient
            {
                Id = _patientId,
                FullName = _validPatientName,
                PhoneNumber = _validPatientPhone,
                Email = _validPatientEmail
            };

            var patients = new List<Patient> { existingPatient }.AsQueryable().BuildMockDbSet();
            _context.Patients.Returns(patients);

            var existingAppointments = new List<Appointment>
            {
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    PatientId = _patientId,
                    Date = new DateTimeOffset(_validFutureDate.Date.AddHours(2)), // Different time, same day
                    Status = AppointmentStatus.Pending
                }
            }.AsQueryable().BuildMockDbSet();

            _context.Appointments.Returns(existingAppointments);

            // Act & Assert
            var result = await _validator.TestValidateAsync(_validCommand);
            result.ShouldHaveValidationErrorFor(x => x)
                .WithErrorMessage(_patientUnavailableMessage);
        }

        [Fact]
        public async Task Validate_WhenDoctorAndPatientAreAvailable_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var emptyAppointments = new List<Appointment>().AsQueryable().BuildMockDbSet();
            _context.Appointments.Returns(emptyAppointments);

            var emptyPatients = new List<Patient>().AsQueryable().BuildMockDbSet();
            _context.Patients.Returns(emptyPatients);

            // Act & Assert
            var result = await _validator.TestValidateAsync(_validCommand);
            result.ShouldNotHaveAnyValidationErrors();
        }

        #endregion
    }
}
