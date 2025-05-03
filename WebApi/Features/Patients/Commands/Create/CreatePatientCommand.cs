using FluentValidation;
using MediatR;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Infrastructure.Persistence;
using WebApi.Common.Localization;
using WebApi.Application.Common.Interfaces;

namespace WebApi.Features.Patients.Commands.Create
{
    public record CreatePatientCommand(
        string Name,
        string Email,
        string Phone
    ) : IRequest<Result<Guid>>;

    public class CreatePatientCommandValidator : AbstractValidator<CreatePatientCommand>
    {
        public CreatePatientCommandValidator(ILocalizationService localizationService)
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

    public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, Result<Guid>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILocalizationService _localizationService;

        public CreatePatientCommandHandler(IApplicationDbContext context, ILocalizationService localizationService)
        {
            _context = context;
            _localizationService = localizationService;
        }

        public async Task<Result<Guid>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
        {
            var patient = new Patient
            {
                FullName = request.Name,
                Email = request.Email,
                PhoneNumber = request.Phone
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(patient.Id, _localizationService.GetLocalizedString(LocalizationKeys.Patients.Created));
        }
    }
}
