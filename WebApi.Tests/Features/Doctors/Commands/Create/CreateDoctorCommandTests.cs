using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Domain.Entities;
using WebApi.Domain.ValueObjects;
using WebApi.Features.Doctors.Commands.Create;
using WebApi.Infrastructure.Persistence;
using Xunit;

namespace WebApi.Tests.Features.Doctors.Commands.Create
{
    public class CreateDoctorCommandTests
    {
        public class CreateDoctorHandlerTests
        {
            private readonly IApplicationDbContext _dbContext; // Change type to ApplicationDbContext
            private readonly ILocalizationService _localizationService;
            private readonly CreateDoctorHandler _handler;

            public CreateDoctorHandlerTests()
            {
                _dbContext = Substitute.For<IApplicationDbContext>(); // Update instantiation to match the correct type
                _localizationService = Substitute.For<ILocalizationService>();
                _handler = new CreateDoctorHandler(_dbContext, _localizationService);
            }

            // Rest of the code remains unchanged
        }

        public class CreateDoctorValidatorTests
        {
            private readonly ILocalizationService _localizationService;
            private readonly CreateDoctorValidator _validator;

            public CreateDoctorValidatorTests()
            {
                _localizationService = Substitute.For<ILocalizationService>();
                _localizationService.GetLocalizedString(Arg.Any<string>()).Returns("Localized error");
                _validator = new CreateDoctorValidator(_localizationService);
            }

            [Fact]
            public void Should_Have_Error_When_FullName_Is_Empty()
            {
                var command = new CreateDoctorCommand(
                    "", "Spec", "Sum", "Bio", "a@b.com", "+1234567890", "http://img",
                    new Address
                    {
                        AddressLine = "L",
                        Street = "S",
                        District = "D",
                        Country = "C",
                        City = "C",
                        ZipCode = "Z"
                    },
                    new List<PersonalLink>());
                var result = _validator.TestValidate(command);
                result.ShouldHaveValidationErrorFor(x => x.FullName)
                    .WithErrorMessage("Localized error");
            }

            [Fact]
            public void Should_Have_Error_When_Email_Is_Invalid()
            {
                var command = new CreateDoctorCommand(
                    "Name", "Spec", "Sum", "Bio", "notanemail", "+1234567890", "http://img",
                    new Address
                    {
                        AddressLine = "L",
                        Street = "S",
                        District = "D",
                        Country = "C",
                        City = "C",
                        ZipCode = "Z"
                    },
                    new List<PersonalLink>());
                var result = _validator.TestValidate(command);
                result.ShouldHaveValidationErrorFor(x => x.Email)
                    .WithErrorMessage("Localized error");
            }

            [Fact]
            public void Should_Have_Error_When_PhoneNumber_Is_Invalid()
            {
                var command = new CreateDoctorCommand(
                    "Name", "Spec", "Sum", "Bio", "a@b.com", "invalid", "http://img",
                    new Address
                    {
                        AddressLine = "L",
                        Street = "S",
                        District = "D",
                        Country = "C",
                        City = "C",
                        ZipCode = "Z"
                    },
                    new List<PersonalLink>());
                var result = _validator.TestValidate(command);
                result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
                    .WithErrorMessage("Localized error");
            }

            [Fact]
            public void Should_Have_Error_When_ImageUrl_Is_Invalid()
            {
                var command = new CreateDoctorCommand(
                    "Name", "Spec", "Sum", "Bio", "a@b.com", "+1234567890", "not-a-url",
                    new Address
                    {
                        AddressLine = "L",
                        Street = "S",
                        District = "D",
                        Country = "C",
                        City = "C",
                        ZipCode = "Z"
                    },
                    new List<PersonalLink>());
                var result = _validator.TestValidate(command);
                result.ShouldHaveValidationErrorFor(x => x.ImageUrl)
                    .WithErrorMessage("Localized error");
            }

            [Fact]
            public void Should_Not_Have_Error_For_Valid_Command()
            {
                var command = new CreateDoctorCommand(
                    "Name", "Spec", "Sum", "Bio", "a@b.com", "+1234567890", "http://img.com",
                    new Address
                    {
                        AddressLine = "L",
                        Street = "S",
                        District = "D",
                        Country = "C",
                        City = "C",
                        ZipCode = "Z"
                    },
                    new List<PersonalLink>());
                var result = _validator.TestValidate(command);
                result.ShouldNotHaveAnyValidationErrors();
            }
        }
    }
}
