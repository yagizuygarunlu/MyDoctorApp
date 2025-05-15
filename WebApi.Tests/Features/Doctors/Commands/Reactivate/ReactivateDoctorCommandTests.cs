using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Domain.Entities;
using WebApi.Features.Doctors.Commands.Reactivate;

namespace WebApi.Tests.Features.Doctors.Commands.Reactivate
{
    public class ReactivateDoctorCommandTests
    {
        public class ReactivateDoctorHandlerTests
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly ILocalizationService _localizationService;
            private readonly ReactivateDoctorHandler _handler;

            public ReactivateDoctorHandlerTests()
            {
                _dbContext = Substitute.For<IApplicationDbContext>();
                _localizationService = Substitute.For<ILocalizationService>();
                _handler = new ReactivateDoctorHandler(_dbContext, _localizationService);
            }

            [Fact]
            public async Task Handle_ShouldSetIsActiveTrue_AndReturnSuccess_WhenDoctorExists()
            {
                // Arrange
                var doctor = new Doctor
                {
                    Id = 1,
                    FullName = "Test Doctor",
                    Biography = string.Empty,
                    Email = string.Empty,
                    ImageUrl = string.Empty,
                    PhoneNumber = string.Empty,
                    Speciality = string.Empty,
                    SummaryInfo = string.Empty,
                    IsActive = false
                };
                var dbSet = Substitute.For<DbSet<Doctor>>();
                dbSet.FindAsync(1).Returns(new ValueTask<Doctor>(doctor));
                _dbContext.Doctors.Returns(dbSet);

                _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
                _localizationService.GetLocalizedString(Arg.Any<string>()).Returns("Reactivated");

                var command = new ReactivateDoctorCommand(1);

                // Act
                var result = await _handler.Handle(command, CancellationToken.None);

                // Assert
                doctor.IsActive.ShouldBeTrue();
                await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
                result.Succeeded.ShouldBeTrue();
                result.IsSuccess.ShouldBeTrue();
                result.Data.ShouldBe(1);
                result.Message.ShouldBe("Reactivated");
            }

            [Fact]
            public async Task Handle_ShouldReturnFailure_WhenDoctorDoesNotExist()
            {
                // Arrange
                var dbSet = Substitute.For<DbSet<Doctor>>();
                dbSet.FindAsync(2).Returns(new ValueTask<Doctor>((Doctor)null));
                _dbContext.Doctors.Returns(dbSet);

                _localizationService.GetLocalizedString(Arg.Any<string>()).Returns("Not found");

                var command = new ReactivateDoctorCommand(2);

                // Act
                var result = await _handler.Handle(command, CancellationToken.None);

                // Assert
                result.Succeeded.ShouldBeFalse();
                result.IsSuccess.ShouldBeFalse();
                result.Error.ShouldBe("Not found");
            }

            [Fact]
            public async Task Handle_WhenSaveChangesThrows_ShouldPropagateException()
            {
                // Arrange
                var doctor = new Doctor
                {
                    Id = 3,
                    FullName = "Test Doctor",
                    Biography = string.Empty,
                    Email = string.Empty,
                    ImageUrl = string.Empty,
                    PhoneNumber = string.Empty,
                    Speciality = string.Empty,
                    SummaryInfo = string.Empty,
                    IsActive = false
                };
                var dbSet = Substitute.For<DbSet<Doctor>>();
                dbSet.FindAsync(3).Returns(new ValueTask<Doctor>(doctor));
                _dbContext.Doctors.Returns(dbSet);

                _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>())
                    .Returns<Task<int>>(x => { throw new DbUpdateException("DB error"); });

                var command = new ReactivateDoctorCommand(3);

                // Act & Assert
                await Should.ThrowAsync<DbUpdateException>(async () =>
                    await _handler.Handle(command, CancellationToken.None)
                );
            }
        }

        public class ReactivateDoctorValidatorTests
        {
            private readonly ILocalizationService _localizationService;
            private readonly ReactivateDoctorValidator _validator;

            public ReactivateDoctorValidatorTests()
            {
                _localizationService = Substitute.For<ILocalizationService>();
                _localizationService.GetLocalizedString(Arg.Any<string>()).Returns("Localized error");
                _validator = new ReactivateDoctorValidator(_localizationService);
            }

            [Fact]
            public void Should_Have_Error_When_Id_Is_Not_Greater_Than_Zero()
            {
                var command = new ReactivateDoctorCommand(0);
                var result = _validator.TestValidate(command);
                result.ShouldHaveValidationErrorFor(x => x.Id)
                    .WithErrorMessage("Localized error");
            }

            [Fact]
            public void Should_Not_Have_Error_For_Valid_Id()
            {
                var command = new ReactivateDoctorCommand(1);
                var result = _validator.TestValidate(command);
                result.ShouldNotHaveAnyValidationErrors();
            }
        }
    }
}
