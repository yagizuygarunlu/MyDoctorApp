using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Domain.Entities;
using WebApi.Features.Patients.Commands.Update;

namespace WebApi.Tests.Features.Patients.Commands.Update
{
    public class UpdatePatientCommandValidatorTests
    {
        private readonly ILocalizationService _localizationService;
        private readonly UpdatePatientCommandValidator _validator;

        public UpdatePatientCommandValidatorTests()
        {
            _localizationService = Substitute.For<ILocalizationService>();
            _localizationService.GetLocalizedString(Arg.Any<string>()).Returns(callInfo => callInfo.Arg<string>());
            _validator = new UpdatePatientCommandValidator(_localizationService);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            var command = new UpdatePatientCommand(Guid.NewGuid(), "", "+1234567890", "test@example.com");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            var command = new UpdatePatientCommand(Guid.NewGuid(), "John Doe", "+1234567890", "");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var command = new UpdatePatientCommand(Guid.NewGuid(), "John Doe", "+1234567890", "not-an-email");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Phone_Is_Empty()
        {
            var command = new UpdatePatientCommand(Guid.NewGuid(), "John Doe", "", "test@example.com");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Phone);
        }

        [Theory]
        [InlineData("12345")]
        [InlineData("phone")]
        [InlineData("++1234567890")]
        public void Should_Have_Error_When_Phone_Is_Invalid(string phone)
        {
            var command = new UpdatePatientCommand(Guid.NewGuid(), "John Doe", phone, "test@example.com");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Phone);
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            var command = new UpdatePatientCommand(Guid.NewGuid(), "John Doe", "+1234567890", "test@example.com");
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    public class UpdatePatientCommandHandlerTests
    {
        private readonly IApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;
        private readonly UpdatePatientCommandHandler _handler;
        private readonly List<Patient> _patients;

        public UpdatePatientCommandHandlerTests()
        {
            _context = Substitute.For<IApplicationDbContext>();
            _localizationService = Substitute.For<ILocalizationService>();
            _patients = new List<Patient>();

            // Setup Patients DbSet
            var patientsDbSet = Substitute.For<DbSet<Patient>, IQueryable<Patient>>();
            patientsDbSet.FindAsync(Arg.Any<object[]>())
                .Returns(callInfo =>
                {
                    var id = (Guid)callInfo.Arg<object[]>()[0];
                    var patient = _patients.Find(p => p.Id == id);
                    return new ValueTask<Patient?>(patient);
                });

            _context.Patients.Returns(patientsDbSet);
            _context.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

            _localizationService.GetLocalizedString(Arg.Any<string>()).Returns(callInfo => callInfo.Arg<string>());

            _handler = new UpdatePatientCommandHandler(_context, _localizationService);
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_When_Patient_Not_Found()
        {
            var command = new UpdatePatientCommand(Guid.NewGuid(), "Jane Doe", "+19876543210", "jane@example.com");
            var result = await _handler.Handle(command, CancellationToken.None);

            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeFalse();
            result.Error.ShouldBe("Patients.NotFound");
        }

        [Fact]
        public async Task Handle_Should_Update_Patient_And_Return_Success()
        {
            var patientId = Guid.NewGuid();
            var patient = new Patient
            {
                Id = patientId,
                FullName = "Old Name",
                PhoneNumber = "+1111111111",
                Email = "old@example.com"
            };
            _patients.Add(patient);

            var command = new UpdatePatientCommand(patientId, "New Name", "+2222222222", "new@example.com");
            var result = await _handler.Handle(command, CancellationToken.None);

            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();
            result.Data.ShouldBe(MediatR.Unit.Value);
            result.Message.ShouldBe("Patients.Updated");

            patient.FullName.ShouldBe("New Name");
            patient.PhoneNumber.ShouldBe("+2222222222");
            patient.Email.ShouldBe("new@example.com");
        }
    }
}
