using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Domain.Entities;
using WebApi.Features.TreatmentFaqs.Commands.Delete;

namespace WebApi.Tests.Features.TreatmentFaqs.Commands.Delete
{
    public class DeleteTreatmentFaqCommandHandlerTests
    {
        private readonly IApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;
        private readonly DeleteTreatmentFaqCommandHandler _handler;

        public DeleteTreatmentFaqCommandHandlerTests()
        {
            _context = Substitute.For<IApplicationDbContext>();
            _localizationService = Substitute.For<ILocalizationService>();
            _handler = new DeleteTreatmentFaqCommandHandler(_context, _localizationService);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenTreatmentFaqNotFound()
        {
            // Arrange
            var command = new DeleteTreatmentFaqCommand(1);

            // Correctly mock FindAsync to return a ValueTask<TreatmentFaq?> instead of EntityEntry
            _context.TreatmentFaqs.FindAsync(new object[] { 1 }, Arg.Any<CancellationToken>())
                .Returns(callInfo => ValueTask.FromResult<TreatmentFaq?>(null));

            _localizationService.GetLocalizedString(Arg.Any<string>()).Returns("Not found");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.ShouldBeFalse();
            result.Error.ShouldBe("Not found");
            _context.TreatmentFaqs.DidNotReceive().Remove(Arg.Any<TreatmentFaq>());
            await _context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldRemoveAndReturnSuccess_WhenTreatmentFaqFound()
        {
            // Arrange
            var treatmentFaq = new TreatmentFaq { Id = 2, Question = "Test Question", Answer = "Test Answer" };
            var command = new DeleteTreatmentFaqCommand(2);

            // Fix: Match the FindAsync call pattern used by the handler (params object[] keyValues)
            // and simplify the Returns call by letting NSubstitute handle ValueTask wrapping.
            _context.TreatmentFaqs.FindAsync(new object[] { command.Id })
                .Returns(treatmentFaq); // NSubstitute will correctly wrap this as ValueTask<TreatmentFaq?>

            _localizationService.GetLocalizedString(LocalizationKeys.TreatmentFaqs.Deleted).Returns("Deleted"); // Be specific with the key if possible

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.ShouldBeTrue();
            result.Message.ShouldBe("Deleted");
            _context.TreatmentFaqs.Received(1).Remove(treatmentFaq);
            await _context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}