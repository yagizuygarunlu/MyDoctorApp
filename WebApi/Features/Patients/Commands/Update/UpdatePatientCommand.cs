using FluentValidation;
using MediatR;
using WebApi.Common.Localization;
using WebApi.Common.Results;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.Patients.Commands.Update
{
    public record UpdatePatientCommand(
        Guid Id,
        string Name,
        string Phone,
        string Email
        ): IRequest<Result<Unit>>;

    public class UpdatePatientCommandValidator : AbstractValidator<UpdatePatientCommand>
    {
        public UpdatePatientCommandValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
               .NotEmpty()
               .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Patients.NameRequired));

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Patients.EmailRequired))
                .EmailAddress()
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Patients.InvalidEmail));

            RuleFor(x => x.Phone)
                .NotEmpty()
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Patients.PhoneRequired))
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Patients.InvalidPhone));
        }
    }

    public sealed class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, Result<Unit>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;

        public UpdatePatientCommandHandler(
            ApplicationDbContext context,
            ILocalizationService localizationService)
        {
            _context = context;
            _localizationService = localizationService;
        }

        public async Task<Result<Unit>> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
        {
            var patient = await _context.Patients.FindAsync(request.Id);
            if (patient == null)
                return Result<Unit>.Failure(_localizationService.GetLocalizedString(LocalizationKeys.Patients.NotFound));

            patient.FullName = request.Name;
            patient.PhoneNumber = request.Phone;
            patient.Email = request.Email;
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value, _localizationService.GetLocalizedString(LocalizationKeys.Patients.Updated));
        }
    }
}
