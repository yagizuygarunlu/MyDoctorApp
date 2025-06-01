using FluentValidation.TestHelper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Features.Treatments.Commands.Update;

namespace WebApi.UnitTests.Features.Treatments.Commands.Update
{
    public class UpdateTreatmentCommandTests
    {
        public class ValidatorTests
        {
            private readonly UpdateTreatmentCommandValidator _validator;
            private readonly ILocalizationService _localizationService;

            public ValidatorTests()
            {
                _localizationService = Substitute.For<ILocalizationService>();
                _localizationService.GetLocalizedString(LocalizationKeys.Treatments.NameRequired)
                    .Returns("Name is required");

                _validator = new UpdateTreatmentCommandValidator(_localizationService);
            }

            [Fact]
            public void Should_Require_Name()
            {
                // Arrange
                var command = new UpdateTreatmentCommand(
                    Id: 1,
                    Name: string.Empty,
                    Description: "Description",
                    Slug: "slug",
                    Images: null);

                // Act
                var result = _validator.TestValidate(command);

                // Assert
                result.ShouldHaveValidationErrorFor(c => c.Name)
                    .WithErrorMessage("Name is required");
            }

            [Fact]
            public void Should_Pass_Validation_When_Valid()
            {
                // Arrange
                var command = new UpdateTreatmentCommand(
                    Id: 1,
                    Name: "Treatment Name",
                    Description: "Description",
                    Slug: "slug",
                    Images: null);

                // Act
                var result = _validator.TestValidate(command);

                // Assert
                result.ShouldNotHaveAnyValidationErrors();
            }
        }

        public class HandlerTests
        {
            private readonly IApplicationDbContext _context;
            private readonly ILocalizationService _localizationService;
            private readonly UpdateTreatmentCommandHandler _handler;

            public HandlerTests()
            {
                _context = Substitute.For<IApplicationDbContext>();
                _localizationService = Substitute.For<ILocalizationService>();

                _localizationService.GetLocalizedString(LocalizationKeys.Treatments.NotFound)
                    .Returns("Treatment not found");
                _localizationService.GetLocalizedString(LocalizationKeys.Treatments.Updated)
                    .Returns("Treatment updated successfully");

                _handler = new UpdateTreatmentCommandHandler(_context, _localizationService);
            }

            [Fact]
            public async Task Handle_WithExistingTreatment_ShouldUpdateAndReturnSuccess()
            {
                // Arrange
                var treatmentId = 1;
                var treatment = new Treatment
                {
                    Id = treatmentId,
                    Name = "Old Name",
                    Description = "Old Description",
                    Slug = "old-slug"
                };

                _context.Treatments.FindAsync(treatmentId)
                    .Returns(treatment);

                _context.SaveChangesAsync(CancellationToken.None)
                    .Returns(1);

                var command = new UpdateTreatmentCommand(
                    Id: treatmentId,
                    Name: "New Name",
                    Description: "New Description",
                    Slug: "new-slug",
                    Images: null);

                // Act
                var result = await _handler.Handle(command, CancellationToken.None);

                // Assert
                result.ShouldBeOfType<Result<int>>();
                result.Succeeded.ShouldBeTrue();
                result.Data.ShouldBe(treatmentId);
                result.Message.ShouldBe("Treatment updated successfully");

                treatment.Name.ShouldBe("New Name");
                treatment.Description.ShouldBe("New Description");
                treatment.Slug.ShouldBe("new-slug");

                await _context.Received(1).SaveChangesAsync(CancellationToken.None);
            }

            [Fact]
            public async Task Handle_WithNonExistingTreatment_ShouldReturnFailure()
            {
                // Arrange
                var treatmentId = 999;

                _context.Treatments.FindAsync(treatmentId)
                    .ReturnsNull();

                var command = new UpdateTreatmentCommand(
                    Id: treatmentId,
                    Name: "New Name",
                    Description: "New Description",
                    Slug: "new-slug",
                    Images: null);

                // Act
                var result = await _handler.Handle(command, CancellationToken.None);

                // Assert
                result.ShouldBeOfType<Result<int>>();
                result.Succeeded.ShouldBeFalse();
                result.Error.ShouldBe("Treatment not found");

                await _context.DidNotReceive().SaveChangesAsync(CancellationToken.None);
            }

            [Fact]
            public async Task UpdateTreatmentImagesAsync_ShouldRemoveExistingAndAddNewImages()
            {
                // Arrange
                var treatmentId = 1;
                var newImages = new List<TreatmentImage>
                {
                    new TreatmentImage
                    {
                        ImageUrl = "new-url-1",
                        Caption = "New Caption 1",
                        DisplayOrder = 1
                    },
                    new TreatmentImage
                    {
                        ImageUrl = "new-url-2",
                        Caption = "New Caption 2",
                        DisplayOrder = 2
                    }
                };

                _context.SaveChangesAsync(CancellationToken.None)
                    .Returns(Task.FromResult(1));

                // Act
                var result = await _handler.UpdateTreatmentImagesAsync(treatmentId, newImages, CancellationToken.None);

                // Assert
                result.ShouldBe(Unit.Value);

                // Verify that new images were added
                _context.TreatmentImages.Received().AddRange(newImages);
                await _context.Received(1).SaveChangesAsync(CancellationToken.None);
            }

            [Fact]
            public async Task UpdateTreatmentImagesAsync_WithNullImages_ShouldNotAddAnyImages()
            {
                // Arrange
                var treatmentId = 1;

                // Act
                var result = await _handler.UpdateTreatmentImagesAsync(treatmentId, null, CancellationToken.None);

                // Assert
                result.ShouldBe(Unit.Value);

                _context.TreatmentImages.DidNotReceive().AddRange(Arg.Any<IEnumerable<TreatmentImage>>());
                await _context.DidNotReceive().SaveChangesAsync(CancellationToken.None);
            }

            [Fact]
            public async Task UpdateTreatmentImagesAsync_WithEmptyImages_ShouldNotAddAnyImages()
            {
                // Arrange
                var treatmentId = 1;
                var emptyImages = new List<TreatmentImage>();

                // Act
                var result = await _handler.UpdateTreatmentImagesAsync(treatmentId, emptyImages, CancellationToken.None);

                // Assert
                result.ShouldBe(Unit.Value);

                _context.TreatmentImages.DidNotReceive().AddRange(emptyImages);
                await _context.DidNotReceive().SaveChangesAsync(CancellationToken.None);
            }
        }
    }
}