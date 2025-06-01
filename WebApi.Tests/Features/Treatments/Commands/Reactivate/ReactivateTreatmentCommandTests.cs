using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Features.Treatments.Commands.Reactivate;

namespace WebApi.Tests.Features.Treatments.Commands.Reactivate
{
    public class ReactivateTreatmentCommandTests
    {
        private readonly IApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;
        private readonly ReactivateTreatmentCommandHandler _handler;

        public ReactivateTreatmentCommandTests()
        {
            _context = Substitute.For<IApplicationDbContext>();
            _localizationService = Substitute.For<ILocalizationService>();

            _localizationService.GetLocalizedString(LocalizationKeys.Treatments.NotFound)
                .Returns("Treatment not found");
            _localizationService.GetLocalizedString(LocalizationKeys.Treatments.Reactivated)
                .Returns("Treatment reactivated successfully");

            _handler = new ReactivateTreatmentCommandHandler(_context, _localizationService);
        }

        [Fact]
        public async Task Handle_WithExistingTreatment_ShouldReactivateAndReturnSuccess()
        {
            // Arrange
            var treatmentId = 1;
            var treatment = new Treatment 
            { 
                Id = treatmentId, 
                Name = "Test Treatment", 
                IsActive = false 
            };

            _context.Treatments.FindAsync(treatmentId)
                .Returns(treatment);

            _context.SaveChangesAsync(CancellationToken.None)
                .Returns(1);

            var command = new ReactivateTreatmentCommand(treatmentId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldBeOfType<Result<int>>();
            result.Succeeded.ShouldBeTrue();
            result.Data.ShouldBe(treatmentId);
            result.Message.ShouldBe("Treatment reactivated successfully");

            treatment.IsActive.ShouldBeTrue();

            await _context.Received(1).SaveChangesAsync(CancellationToken.None);
        }

        [Fact]
        public async Task Handle_WithNonExistingTreatment_ShouldReturnFailure()
        {
            // Arrange
            var treatmentId = 999;

            _context.Treatments.FindAsync(treatmentId)
                .Returns(ValueTask.FromResult<Treatment?>(null));

            var command = new ReactivateTreatmentCommand(treatmentId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldBeOfType<Result<int>>();
            result.Succeeded.ShouldBeFalse();
            result.Error.ShouldBe("Treatment not found");

            await _context.DidNotReceive().SaveChangesAsync(CancellationToken.None);
        }

        [Fact]
        public void Command_ShouldContainCorrectProperties()
        {
            // Arrange & Act
            var treatmentId = 5;
            var command = new ReactivateTreatmentCommand(treatmentId);

            // Assert
            command.Id.ShouldBe(treatmentId);
        }

        [Fact]
        public async Task Handle_ShouldUpdateIsActivePropertyOnly()
        {
            // Arrange
            var treatmentId = 1;
            var originalName = "Original Treatment Name";
            var originalDescription = "Original Description";
            
            var treatment = new Treatment 
            { 
                Id = treatmentId, 
                Name = originalName,
                Description = originalDescription,
                IsActive = false 
            };

            _context.Treatments.FindAsync(treatmentId)
                .Returns(treatment);

            _context.SaveChangesAsync(CancellationToken.None)
                .Returns(1);

            var command = new ReactivateTreatmentCommand(treatmentId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.ShouldBeTrue();
            
            // Verify only IsActive was changed
            treatment.IsActive.ShouldBeTrue();
            treatment.Name.ShouldBe(originalName);
            treatment.Description.ShouldBe(originalDescription);
        }
    }
} 