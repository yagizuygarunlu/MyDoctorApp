using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Domain.Entities;
using WebApi.Features.Patients.Commands.Create;

namespace WebApi.Tests.Features.Patients.Commands.Create
{
    public class CreatePatientCommandValidatorTests
    {
        private readonly ILocalizationService _localizationService;
        private readonly CreatePatientCommandValidator _validator;

        public CreatePatientCommandValidatorTests()
        {
            _localizationService = Substitute.For<ILocalizationService>();
            _localizationService.GetLocalizedString(Arg.Any<string>()).Returns(callInfo => callInfo.Arg<string>());
            _validator = new CreatePatientCommandValidator(_localizationService);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            var command = new CreatePatientCommand("", "test@example.com", "+1234567890");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            var command = new CreatePatientCommand("John Doe", "", "+1234567890");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var command = new CreatePatientCommand("John Doe", "not-an-email", "+1234567890");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Phone_Is_Empty()
        {
            var command = new CreatePatientCommand("John Doe", "test@example.com", "");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Phone);
        }

        [Theory]
        [InlineData("12345")]
        [InlineData("phone")]
        [InlineData("++1234567890")]
        public void Should_Have_Error_When_Phone_Is_Invalid(string phone)
        {
            var command = new CreatePatientCommand("John Doe", "test@example.com", phone);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Phone);
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            var command = new CreatePatientCommand("John Doe", "test@example.com", "+1234567890");
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    public class CreatePatientCommandHandlerTests
    {
        private readonly IApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;
        private readonly CreatePatientCommandHandler _handler;
        private readonly List<Patient> _patients;

        public CreatePatientCommandHandlerTests()
        {
            _context = Substitute.For<IApplicationDbContext>();
            _localizationService = Substitute.For<ILocalizationService>();
            _patients = new List<Patient>();

            var patientsDbSet = Substitute.For<DbSet<Patient>, IQueryable<Patient>>();
            
            // Fix: Create the patient with a non-empty GUID when added
            patientsDbSet.Add(Arg.Do<Patient>(p => {
                p.Id = Guid.NewGuid(); // Generate a non-empty GUID
                _patients.Add(p);
            }));

            _context.Patients.Returns(patientsDbSet);
            _context.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

            _localizationService.GetLocalizedString(Arg.Any<string>()).Returns("Patient created");

            _handler = new CreatePatientCommandHandler(_context, _localizationService);
        }

        [Fact]
        public async Task Handle_Should_Add_Patient_And_Return_Success()
        {
            var command = new CreatePatientCommand("Jane Doe", "jane@example.com", "+19876543210");
            var result = await _handler.Handle(command, CancellationToken.None);

            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();
            result.Data.ShouldNotBe(Guid.Empty);
            result.Message.ShouldBe("Patient created");

            _patients.Count.ShouldBe(1);
            var patient = _patients[0];
            patient.FullName.ShouldBe("Jane Doe");
            patient.Email.ShouldBe("jane@example.com");
            patient.PhoneNumber.ShouldBe("+19876543210");
        }
    }
}
