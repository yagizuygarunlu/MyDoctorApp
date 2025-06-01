using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Features.TreatmentFaqs.Commands.Delete;

namespace WebApi.Tests.Features.TreatmentFaqs.Commands.Delete
{
    public class DeleteTreatmentFaqCommandTests
    {
        private readonly IApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;
        private readonly DeleteTreatmentFaqCommandHandler _handler;

        public DeleteTreatmentFaqCommandTests()
        {
            _context = Substitute.For<IApplicationDbContext>();
            _localizationService = Substitute.For<ILocalizationService>();

            _localizationService.GetLocalizedString(LocalizationKeys.TreatmentFaqs.NotFound)
                .Returns("FAQ not found");
            _localizationService.GetLocalizedString(LocalizationKeys.TreatmentFaqs.Deleted)
                .Returns("FAQ deleted successfully");

            _handler = new DeleteTreatmentFaqCommandHandler(_context, _localizationService);
        }

        [Fact]
        public async Task Handle_WithExistingFaq_ShouldDeleteAndReturnSuccess()
        {
            // Arrange
            var faqId = 1;
            var faq = new TreatmentFaq 
            { 
                Id = faqId, 
                Question = "Test Question",
                Answer = "Test Answer",
                TreatmentId = 1
            };

            _context.TreatmentFaqs.FindAsync(faqId)
                .Returns(faq);

            _context.SaveChangesAsync(CancellationToken.None)
                .Returns(1);

            var command = new DeleteTreatmentFaqCommand(faqId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldBeOfType<Result<Unit>>();
            result.Succeeded.ShouldBeTrue();
            result.Data.ShouldBe(Unit.Value);
            result.Message.ShouldBe("FAQ deleted successfully");

            _context.TreatmentFaqs.Received(1).Remove(faq);
            await _context.Received(1).SaveChangesAsync(CancellationToken.None);
        }

        [Fact]
        public async Task Handle_WithNonExistingFaq_ShouldReturnFailure()
        {
            // Arrange
            var faqId = 999;

            _context.TreatmentFaqs.FindAsync(faqId)
                .Returns(ValueTask.FromResult<TreatmentFaq?>(null));

            var command = new DeleteTreatmentFaqCommand(faqId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldBeOfType<Result<Unit>>();
            result.Succeeded.ShouldBeFalse();
            result.Error.ShouldBe("FAQ not found");

            _context.TreatmentFaqs.DidNotReceive().Remove(Arg.Any<TreatmentFaq>());
            await _context.DidNotReceive().SaveChangesAsync(CancellationToken.None);
        }

        [Fact]
        public void Command_ShouldContainCorrectProperties()
        {
            // Arrange & Act
            var faqId = 5;
            var command = new DeleteTreatmentFaqCommand(faqId);

            // Assert
            command.Id.ShouldBe(faqId);
        }

        [Fact]
        public async Task Handle_ShouldRemoveFaqFromContext()
        {
            // Arrange
            var faqId = 2;
            var faq = new TreatmentFaq 
            { 
                Id = faqId, 
                Question = "Another Question",
                Answer = "Another Answer",
                TreatmentId = 2
            };

            _context.TreatmentFaqs.FindAsync(faqId)
                .Returns(faq);

            _context.SaveChangesAsync(CancellationToken.None)
                .Returns(1);

            var command = new DeleteTreatmentFaqCommand(faqId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.ShouldBeTrue();
            
            // Verify the exact FAQ entity was removed
            _context.TreatmentFaqs.Received(1).Remove(faq);
        }

        [Fact]
        public async Task Handle_WhenSaveChangesFails_ShouldStillReturnSuccess()
        {
            // Arrange
            var faqId = 3;
            var faq = new TreatmentFaq 
            { 
                Id = faqId, 
                Question = "Test Question",
                Answer = "Test Answer",
                TreatmentId = 1
            };

            _context.TreatmentFaqs.FindAsync(faqId)
                .Returns(faq);

            _context.SaveChangesAsync(CancellationToken.None)
                .Returns(0); // Simulate no changes saved

            var command = new DeleteTreatmentFaqCommand(faqId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.ShouldBeTrue();
            result.Message.ShouldBe("FAQ deleted successfully");
            
            // Verify remove was still called
            _context.TreatmentFaqs.Received(1).Remove(faq);
        }
    }
} 