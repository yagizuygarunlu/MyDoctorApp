using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Features.Treatments.Commands.Create;
using Xunit;

namespace WebApi.UnitTests.Features.Treatments.Commands.Create
{
    public class CreateTreatmentCommandTests
    {
        public class ValidatorTests
        {
            private readonly ILocalizationService _localizationService;
            private readonly CreateTreatmentCommandValidator _validator;

            public ValidatorTests()
            {
                _localizationService = Substitute.For<ILocalizationService>();
                _localizationService.GetLocalizedString(LocalizationKeys.Treatments.NameRequired)
                    .Returns("Name is required");

                _validator = new CreateTreatmentCommandValidator(_localizationService);
            }

            [Fact]
            public void Should_Require_Name()
            {
                // Arrange
                var command = new CreateTreatmentCommand(
                    Name: string.Empty,
                    Description: "Description",
                    Slug: "slug",
                    Images: null);

                // Act
                var result = _validator.Validate(command);

                // Assert
                result.IsValid.ShouldBeFalse();
                result.Errors.Count.ShouldBe(1);
                result.Errors[0].ErrorMessage.ShouldBe("Name is required");
            }

            [Fact]
            public void Should_Pass_Validation_When_Valid()
            {
                // Arrange
                var command = new CreateTreatmentCommand(
                    Name: "Treatment Name",
                    Description: "Description",
                    Slug: "slug",
                    Images: null);

                // Act
                var result = _validator.Validate(command);

                // Assert
                result.IsValid.ShouldBeTrue();
            }
        }

        public class HandlerTests
        {
            private readonly IApplicationDbContext _context;
            private readonly ILocalizationService _localizationService;
            private readonly CreateTreatmentCommandHandler _handler;

            public HandlerTests()
            {
                _context = Substitute.For<IApplicationDbContext>();
                _localizationService = Substitute.For<ILocalizationService>();

                _localizationService.GetLocalizedString(LocalizationKeys.Treatments.Created)
                    .Returns("Treatment created successfully");

                _handler = new CreateTreatmentCommandHandler(_context, _localizationService);
            }

            [Fact]
            public async Task Handle_ShouldCreateTreatment_AndReturnSuccessResult()
            {
                // Arrange
                var command = new CreateTreatmentCommand(
                    Name: "Test Treatment",
                    Description: "Test Description",
                    Slug: "test-treatment",
                    Images: null);

                Treatment savedTreatment = null;
                _context.Treatments.Add(Arg.Do<Treatment>(t =>
                {
                    savedTreatment = t;
                    savedTreatment.Id = 1;
                }));

                _context.SaveChangesAsync(Arg.Any<CancellationToken>())
                    .Returns(1);

                // Act
                var result = await _handler.Handle(command, CancellationToken.None);

                // Assert
                result.ShouldBeOfType<Result<int>>();
                result.Succeeded.ShouldBeTrue();
                result.Data.ShouldBe(1);
                result.Message.ShouldBe("Treatment created successfully");

                savedTreatment.ShouldNotBeNull();
                savedTreatment.Name.ShouldBe(command.Name);
                savedTreatment.Description.ShouldBe(command.Description);
                savedTreatment.Slug.ShouldBe(command.Slug);

                await _context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
            }

            [Fact]
            public async Task Handle_WithImages_ShouldCreateTreatmentWithImages()
            {
                // Arrange
                var images = new List<TreatmentImage>
                {
                    new TreatmentImage
                    {
                        ImageUrl = "url1",
                        Caption = "caption1",
                        DisplayOrder = 1
                    },
                    new TreatmentImage
                    {
                        ImageUrl = "url2",
                        Caption = "caption2",
                        DisplayOrder = 2
                    }
                };

                var command = new CreateTreatmentCommand(
                    Name: "Test Treatment",
                    Description: "Test Description",
                    Slug: "test-treatment",
                    Images: images);

                Treatment savedTreatment = null;
                _context.Treatments.Add(Arg.Do<Treatment>(t =>
                {
                    savedTreatment = t;
                    savedTreatment.Id = 1;
                }));

                _context.SaveChangesAsync(Arg.Any<CancellationToken>())
                    .Returns(1);

                // Act
                var result = await _handler.Handle(command, CancellationToken.None);

                // Assert
                result.Succeeded.ShouldBeTrue();
                result.Data.ShouldBe(1);

                savedTreatment.ShouldNotBeNull();

                _context.TreatmentImages.Received(1).AddRange(Arg.Is<IEnumerable<TreatmentImage>>(
                    imgs => imgs.Count() == 2 && imgs.All(img => img.TreatmentId == 1)));

                await _context.Received(2).SaveChangesAsync(Arg.Any<CancellationToken>());
            }

            [Fact]
            public async Task AddTreatmentImagesAsync_WithNullImages_ShouldReturnUnit()
            {
                // Arrange
                List<TreatmentImage>? images = null;
                const int treatmentId = 1;

                // Act
                var result = await _handler.AddTreatmentImagesAsync(images, treatmentId, CancellationToken.None);

                // Assert
                result.ShouldBe(Unit.Value);
                _context.TreatmentImages.DidNotReceive().AddRange(Arg.Any<IEnumerable<TreatmentImage>>());
                await _context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
            }

            [Fact]
            public async Task AddTreatmentImagesAsync_WithEmptyImages_ShouldReturnUnit()
            {
                // Arrange
                var images = new List<TreatmentImage>();
                const int treatmentId = 1;

                // Act
                var result = await _handler.AddTreatmentImagesAsync(images, treatmentId, CancellationToken.None);

                // Assert
                result.ShouldBe(Unit.Value);
                _context.TreatmentImages.DidNotReceive().AddRange(Arg.Any<IEnumerable<TreatmentImage>>());
                await _context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
            }

            [Fact]
            public async Task AddTreatmentImagesAsync_WithImages_ShouldAddTreatmentImages()
            {
                // Arrange
                var images = new List<TreatmentImage>
                {
                    new TreatmentImage
                    {
                        ImageUrl = "url1",
                        Caption = "caption1",
                        DisplayOrder = 1
                    },
                    new TreatmentImage
                    {
                        ImageUrl = "url2",
                        Caption = "caption2",
                        DisplayOrder = 2
                    }
                };

                const int treatmentId = 1;
                List<TreatmentImage> addedImages = null;

                _context.TreatmentImages.AddRange(Arg.Do<IEnumerable<TreatmentImage>>(imgs =>
                    addedImages = imgs.ToList()));

                _context.SaveChangesAsync(Arg.Any<CancellationToken>())
                    .Returns(1);

                // Act
                var result = await _handler.AddTreatmentImagesAsync(images, treatmentId, CancellationToken.None);

                // Assert
                result.ShouldBe(Unit.Value);

                addedImages.ShouldNotBeNull();
                addedImages.Count.ShouldBe(2);
                addedImages.All(img => img.TreatmentId == treatmentId).ShouldBeTrue();

                await _context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
            }
        }
    }
}