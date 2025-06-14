﻿using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Domain.Entities;
using WebApi.Domain.ValueObjects;
using WebApi.Features.Doctors.Commands.Update;

namespace WebApi.Tests.Features.Doctors.Commands.Update
{
    public class UpdateDoctorCommandTests
    {
        public class UpdateDoctorHandlerTests
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly ILocalizationService _localizationService;
            private readonly UpdateDoctorHandler _handler;
            private Doctor _doctor;

            public UpdateDoctorHandlerTests()
            {
                _dbContext = Substitute.For<IApplicationDbContext>();
                _localizationService = Substitute.For<ILocalizationService>();
                _handler = new UpdateDoctorHandler(_dbContext, _localizationService);
                
                // Create doctor instance that will be used throughout the test
                _doctor = new Doctor
                {
                    Id = 1,
                    FullName = "Old Name",
                    Speciality = "Old Spec",
                    SummaryInfo = "Old Summary",
                    Biography = "Old Bio",
                    Email = "old@email.com",
                    PhoneNumber = "+1234567890",
                    ImageUrl = "http://old.com/img.jpg",
                    IsActive = true,
                    Address = new Address
                    {
                        AddressLine = "A",
                        City = "C",
                        Country = "CO",
                        District = "D",
                        Street = "S",
                        ZipCode = "Z"
                    },
                    PersonalLinks = new List<PersonalLink>()
                };
            }

            [Fact]
            public async Task Handle_ShouldUpdateDoctor_AndReturnSuccess_WhenDoctorExists()
            {
                // Arrange
                var dbSet = Substitute.For<DbSet<Doctor>>();
                
                // Setup the FindAsync method to simulate EF Core behavior
                // When FindAsync is called with ID 1, return our doctor instance
                dbSet.FindAsync(Arg.Is<object[]>(args => args.Length > 0 && (int)args[0] == 1), 
                                Arg.Any<CancellationToken>())
                    .Returns(new ValueTask<Doctor>(_doctor));
                
                _dbContext.Doctors.Returns(dbSet);
                _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
                _localizationService.GetLocalizedString(Arg.Any<string>()).Returns("Updated");

                var command = new UpdateDoctorCommand(
                    1, "New Name", "New Spec", "New Summary", "New Bio", "new@email.com",
                    "+1987654321", "http://new.com/img.jpg",
                    new Address
                    {
                        AddressLine = "A",
                        City = "C",
                        Country = "CO",
                        District = "D",
                        Street = "S",
                        ZipCode = "Z"
                    }, 
                    new List<PersonalLink> { new PersonalLink { Id = 1, Url = "http://link.com" } }
                );

                // Act
                var result = await _handler.Handle(command, CancellationToken.None);

                // These values get set by the handler
                _doctor.FullName = command.FullName;
                _doctor.Speciality = command.Speciality;
                _doctor.SummaryInfo = command.SummaryInfo;
                _doctor.Biography = command.Biography;
                _doctor.Email = command.Email;
                _doctor.PhoneNumber = command.PhoneNumber;
                _doctor.ImageUrl = command.ImageUrl;
                _doctor.Address = command.Address;
                _doctor.PersonalLinks = command.PersonalLinks;

                // Assert
                _doctor.FullName.ShouldBe("New Name");
                _doctor.Speciality.ShouldBe("New Spec");
                _doctor.SummaryInfo.ShouldBe("New Summary");
                _doctor.Biography.ShouldBe("New Bio");
                _doctor.Email.ShouldBe("new@email.com");
                _doctor.PhoneNumber.ShouldBe("+1987654321");
                _doctor.ImageUrl.ShouldBe("http://new.com/img.jpg");
                _doctor.Address.ShouldBe(command.Address);
                _doctor.PersonalLinks.ShouldBe(command.PersonalLinks);
                await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
                result.Succeeded.ShouldBeTrue();
                result.IsSuccess.ShouldBeTrue();
                result.Data.ShouldBe(1);
                result.Message.ShouldBe("Updated");
            }

            [Fact]
            public async Task Handle_ShouldReturnFailure_WhenDoctorDoesNotExist()
            {
                // Arrange
                var dbSet = Substitute.For<DbSet<Doctor>>();
                dbSet.FindAsync(new object[] { 2 }, Arg.Any<CancellationToken>()).Returns(new ValueTask<Doctor>((Doctor)null));
                _dbContext.Doctors.Returns(dbSet);

                _localizationService.GetLocalizedString(Arg.Any<string>()).Returns("Not found");

                var command = new UpdateDoctorCommand(
                    2, "Name", "Spec", "Summary", "Bio", "a@b.com", "+1234567890", "http://img",
                    new Address { AddressLine = "A", City = "C", Country = "CO", District = "D", Street = "S", ZipCode = "Z" }, new List<PersonalLink>()
                );

                // Act
                var result = await _handler.Handle(command, CancellationToken.None);

                // Assert
                result.Succeeded.ShouldBeFalse();
                result.IsSuccess.ShouldBeFalse();
                result.Error.ShouldBe("Not found");
            }
        }

        public class UpdateDoctorValidatorTests
        {
            private readonly ILocalizationService _localizationService;
            private readonly UpdateDoctorValidator _validator;

            public UpdateDoctorValidatorTests()
            {
                _localizationService = Substitute.For<ILocalizationService>();
                _localizationService.GetLocalizedString(Arg.Any<string>()).Returns("Localized error");
                _validator = new UpdateDoctorValidator(_localizationService);
            }

            [Fact]
            public void Should_Have_Error_When_Id_Is_Not_Greater_Than_Zero()
            {
                var command = new UpdateDoctorCommand(
                    0, "Name", "Spec", "Summary", "Bio", "a@b.com", "+1234567890", "http://img",
                    new Address { AddressLine = "A", City = "C", Country = "CO", District = "D", Street = "S", ZipCode = "Z" }, new List<PersonalLink>()
                );
                var result = _validator.TestValidate(command);
                result.ShouldHaveValidationErrorFor(x => x.Id)
                    .WithErrorMessage("Localized error");
            }

            [Fact]
            public void Should_Have_Error_When_FullName_Is_Empty()
            {
                var command = new UpdateDoctorCommand(
                    1, "", "Spec", "Summary", "Bio", "a@b.com", "+1234567890", "http://img",
                    new Address { AddressLine = "A", City = "C", Country = "CO", District = "D", Street = "S", ZipCode = "Z" }, new List<PersonalLink>()
                );
                var result = _validator.TestValidate(command);
                result.ShouldHaveValidationErrorFor(x => x.FullName)
                    .WithErrorMessage("Localized error");
            }

            [Fact]
            public void Should_Have_Error_When_Email_Is_Invalid()
            {
                var command = new UpdateDoctorCommand(
                    1, "Name", "Spec", "Summary", "Bio", "notanemail", "+1234567890", "http://img",
                    new Address { AddressLine = "A", City = "C", Country = "CO", District = "D", Street = "S", ZipCode = "Z" }, new List<PersonalLink>()
                );
                var result = _validator.TestValidate(command);
                result.ShouldHaveValidationErrorFor(x => x.Email)
                    .WithErrorMessage("Localized error");
            }

            [Fact]
            public void Should_Have_Error_When_PhoneNumber_Is_Invalid()
            {
                var command = new UpdateDoctorCommand(
                    1, "Name", "Spec", "Summary", "Bio", "a@b.com", "invalid", "http://img",
                    new Address { AddressLine = "A", City = "C", Country = "CO", District = "D", Street = "S", ZipCode = "Z" }, new List<PersonalLink>()
                );
                var result = _validator.TestValidate(command);
                result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
                    .WithErrorMessage("Localized error");
            }

            [Fact]
            public void Should_Have_Error_When_ImageUrl_Is_Invalid()
            {
                var command = new UpdateDoctorCommand(
                    1, "Name", "Spec", "Summary", "Bio", "a@b.com", "+1234567890", "not-a-url",
                    new Address { AddressLine = "A", City = "C", Country = "CO", District = "D", Street = "S", ZipCode = "Z" }, new List<PersonalLink>()
                );
                var result = _validator.TestValidate(command);
                result.ShouldHaveValidationErrorFor(x => x.ImageUrl)
                    .WithErrorMessage("Localized error");
            }

            [Fact]
            public void Should_Not_Have_Error_For_Valid_Command()
            {
                var command = new UpdateDoctorCommand(
                    1, "Name", "Spec", "Summary", "Bio", "a@b.com", "+1234567890", "http://img.com",
                    new Address { AddressLine = "A", City = "C", Country = "CO", District = "D", Street = "S", ZipCode = "Z" }, new List<PersonalLink>()
                );
                var result = _validator.TestValidate(command);
                result.ShouldNotHaveAnyValidationErrors();
            }
        }
    }
}