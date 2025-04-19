using FluentValidation;
using MediatR;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Domain.ValueObjects;
using WebApi.Infrastructure.Persistence;
using WebApi.Common.Localization;

namespace WebApi.Features.Doctors.Commands.Update
{
    public record UpdateDoctorCommand(
        int Id,
        string FullName,
        string Speciality,
        string SummaryInfo,
        string Biography,
        string Email,
        string PhoneNumber,
        string ImageUrl,
        Address Address,
        ICollection<PersonalLink> PersonalLinks
    ) : IRequest<Result<int>>;

    public class UpdateDoctorValidator : AbstractValidator<UpdateDoctorCommand>
    {
        public UpdateDoctorValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Doctors.InvalidId));
            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Doctors.FullNameRequired))
                .MaximumLength(100)
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Doctors.FullNameMaxLength));
            RuleFor(x => x.Speciality)
                .NotEmpty()
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Doctors.SpecialityRequired))
                .MaximumLength(50)
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Doctors.SpecialityMaxLength));
            RuleFor(x => x.SummaryInfo)
                .NotEmpty()
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Doctors.SummaryInfoRequired))
                .MaximumLength(500)
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Doctors.SummaryInfoMaxLength));
            RuleFor(x => x.Biography)
                .NotEmpty()
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Doctors.BiographyRequired))
                .MaximumLength(2000)
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Doctors.BiographyMaxLength));
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Doctors.EmailRequired))
                .EmailAddress()
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Doctors.InvalidEmail));
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Doctors.PhoneNumberRequired))
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Doctors.InvalidPhoneNumber));
            RuleFor(x => x.ImageUrl)
                .NotEmpty()
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Doctors.ImageUrlRequired))
                .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Doctors.InvalidImageUrl));
        }
    }

    public sealed class UpdateDoctorHandler : IRequestHandler<UpdateDoctorCommand, Result<int>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILocalizationService _localizationService;

        public UpdateDoctorHandler(ApplicationDbContext dbContext, ILocalizationService localizationService)
        {
            _dbContext = dbContext;
            _localizationService = localizationService;
        }

        public async Task<Result<int>> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
        {
            var doctor = await _dbContext.Doctors.FindAsync(new object[] { request.Id }, cancellationToken);
            if (doctor == null)
            {
                return Result<int>.Failure(_localizationService.GetLocalizedString(LocalizationKeys.Doctors.NotFound));
            }

            doctor.FullName = request.FullName;
            doctor.Speciality = request.Speciality;
            doctor.SummaryInfo = request.SummaryInfo;
            doctor.Biography = request.Biography;
            doctor.Email = request.Email;
            doctor.PhoneNumber = request.PhoneNumber;
            doctor.ImageUrl = request.ImageUrl;
            doctor.Address = request.Address;
            doctor.PersonalLinks = request.PersonalLinks;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(doctor.Id, _localizationService.GetLocalizedString(LocalizationKeys.Doctors.Updated));
        }
    }
}
