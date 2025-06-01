using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Features.Treatments.Commands.Delete;

namespace WebApi.UnitTests.Features.Treatments.Commands.Delete
{
    public class DeleteTreatmentCommandTests
    {
        private readonly IApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;
        private readonly DeleteTreatmentCommandHandler _handler;

        public DeleteTreatmentCommandTests()
        {
            _context = Substitute.For<IApplicationDbContext>();
            _localizationService = Substitute.For<ILocalizationService>();

            _localizationService.GetLocalizedString(LocalizationKeys.Treatments.NotFound)
                .Returns("Treatment not found");

            _localizationService.GetLocalizedString(LocalizationKeys.Treatments.Deleted)
                .Returns("Treatment deleted successfully");

            _handler = new DeleteTreatmentCommandHandler(_context, _localizationService);
        }

        [Fact]
        public async Task Handle_WithExistingTreatment_ShouldMarkAsInactiveAndReturnSuccess()
        {
            // Arrange
            var treatmentId = 1;
            var treatment = new Treatment { Id = treatmentId, Name = "Test Treatment", IsActive = true };

            var dbSetMock = Substitute.For<DbSet<Treatment>>();
            _context.Treatments.Returns(dbSetMock);

            _context.Treatments.FindAsync(treatmentId)
                .Returns(ValueTask.FromResult(treatment));

            _context.SaveChangesAsync(Arg.Any<CancellationToken>())
                .Returns(1);

            var command = new DeleteTreatmentCommand(treatmentId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldBeOfType<Result<Unit>>();
            result.Succeeded.ShouldBeTrue();
            result.Data.ShouldBe(Unit.Value);
            result.Message.ShouldBe("Treatment deleted successfully");

            treatment.IsActive.ShouldBeFalse();
            await _context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WithNonExistingTreatment_ShouldReturnFailure()
        {
            // Arrange
            var treatmentId = 999;

            _context.Treatments.FindAsync(treatmentId)
                .Returns(ValueTask.FromResult<Treatment>(null));

            var command = new DeleteTreatmentCommand(treatmentId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldBeOfType<Result<Unit>>();
            result.Succeeded.ShouldBeFalse();
            result.Error.ShouldBe("Treatment not found");

            await _context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}