using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Features.TreatmentFaqs.Commands.Update;
using Xunit;

namespace WebApi.Tests.Features.TreatmentFaqs.Commands.Update
{
    public class UpdateTreatmentFaqCommandValidatorTests
    {
        private readonly ILocalizationService _localizationService;
        private readonly UpdateTreatmentFaqCommandValidator _validator;

        public UpdateTreatmentFaqCommandValidatorTests()
        {
            _localizationService = Substitute.For<ILocalizationService>();
            _localizationService.GetLocalizedString(Arg.Any<string>()).Returns(callInfo => callInfo.Arg<string>());
            _validator = new UpdateTreatmentFaqCommandValidator(_localizationService);
        }

        [Fact]
        public void Should_Have_Error_When_Question_Is_Empty()
        {
            var command = new UpdateTreatmentFaqCommand(1, "", "Some answer", 1);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Question);
        }

        [Fact]
        public void Should_Have_Error_When_Answer_Is_Empty()
        {
            var command = new UpdateTreatmentFaqCommand(1, "Some question", "", 1);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Answer);
        }

        [Fact]
        public void Should_Have_Error_When_TreatmentId_Is_Zero()
        {
            var command = new UpdateTreatmentFaqCommand(1, "Some question", "Some answer", 0);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.TreatmentId);
        }

        [Fact]
        public void Should_Have_Error_When_TreatmentId_Is_Negative()
        {
            var command = new UpdateTreatmentFaqCommand(1, "Some question", "Some answer", -5);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.TreatmentId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            var command = new UpdateTreatmentFaqCommand(1, "Some question", "Some answer", 2);
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    public class UpdateTreatmentFaqCommandHandlerTests
    {
        private readonly IApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;
        private readonly UpdateTreatmentFaqCommandHandler _handler;
        private readonly TreatmentFaq _existingFaq;

        public UpdateTreatmentFaqCommandHandlerTests()
        {
            _context = Substitute.For<IApplicationDbContext>();
            _localizationService = Substitute.For<ILocalizationService>();
            
            // Create an existing FAQ for testing
            _existingFaq = new TreatmentFaq
            {
                Id = 1,
                Question = "Original question",
                Answer = "Original answer",
                TreatmentId = 1
            };

            // Setup TreatmentFaqs DbSet FindAsync method
            var treatmentFaqsDbSet = Substitute.For<DbSet<TreatmentFaq>>();
            treatmentFaqsDbSet.FindAsync(Arg.Any<object[]>())
                .Returns(callInfo =>
                {
                    var id = (int)callInfo.Arg<object[]>()[0];
                    return id == _existingFaq.Id 
                        ? new ValueTask<TreatmentFaq?>(_existingFaq) 
                        : new ValueTask<TreatmentFaq?>(result: null);
                });

            _context.TreatmentFaqs.Returns(treatmentFaqsDbSet);
            _context.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

            _localizationService.GetLocalizedString(Arg.Any<string>())
                .Returns(callInfo => callInfo.Arg<string>());

            _handler = new UpdateTreatmentFaqCommandHandler(_context, _localizationService);
        }

        [Fact]
        public async Task Handle_Should_Return_NotFound_When_TreatmentFaq_Does_Not_Exist()
        {
            // Use a non-existent ID
            var command = new UpdateTreatmentFaqCommand(99, "Updated question", "Updated answer", 2);
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeFalse();
            result.Error.ShouldBe("TreatmentFaqs.NotFound");
        }

        [Fact]
        public async Task Handle_Should_Update_TreatmentFaq_And_Return_Success()
        {
            var command = new UpdateTreatmentFaqCommand(1, "Updated question", "Updated answer", 2);
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();
            result.Data.ShouldBe(Unit.Value);
            result.Message.ShouldBe("TreatmentFaqs.Updated");

            // Verify the entity was updated
            _existingFaq.Question.ShouldBe("Updated question");
            _existingFaq.Answer.ShouldBe("Updated answer");
            _existingFaq.TreatmentId.ShouldBe(2);

            // Verify SaveChangesAsync was called
            await _context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
