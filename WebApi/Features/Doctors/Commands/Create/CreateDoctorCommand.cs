using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Domain.ValueObjects;

namespace WebApi.Features.Doctors.Commands.Create
{
    public record CreateDoctorCommand
        (
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

    public class CreateDoctorValidator : AbstractValidator<CreateDoctorCommand>
    {
        private readonly ILocalizationService _localizationService;

        public CreateDoctorValidator(ILocalizationService localizationService)
        {
            _localizationService = localizationService;

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Doctors.FullNameRequired))
                .MaximumLength(100).WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Doctors.FullNameMaxLength));
            RuleFor(x => x.Speciality)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Doctors.SpecialityRequired))
                .MaximumLength(50).WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Doctors.SpecialityMaxLength));
            RuleFor(x => x.SummaryInfo)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Doctors.SummaryInfoRequired))
                .MaximumLength(500).WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Doctors.SummaryInfoMaxLength));
            RuleFor(x => x.Biography)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Doctors.BiographyRequired))
                .MaximumLength(2000).WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Doctors.BiographyMaxLength));
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Doctors.EmailRequired))
                .EmailAddress().WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Doctors.InvalidEmail));
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Doctors.PhoneNumberRequired))
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Doctors.InvalidPhoneNumber));
            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Doctors.ImageUrlRequired))
                .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute)).WithMessage(_localizationService.GetLocalizedString(LocalizationKeys.Doctors.InvalidImageUrl));
        }
    }

    public sealed class CreateDoctorHandler : IRequestHandler<CreateDoctorCommand, Result<int>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILocalizationService _localizationService;
        public CreateDoctorHandler(IApplicationDbContext dbContext, ILocalizationService localizationService)
        {
            _dbContext = dbContext;
            _localizationService = localizationService;
        }
        public async Task<Result<int>> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
        {
            var emailExists = await _dbContext.Doctors
             .AnyAsync(d => d.Email == request.Email, cancellationToken);

            if (emailExists)
                return Result<int>.Conflict(_localizationService.GetLocalizedString(LocalizationKeys.Doctors.EmailAlreadyExists));
            
            var doctor = new Doctor
            {
                FullName = request.FullName,
                Speciality = request.Speciality,
                SummaryInfo = request.SummaryInfo,
                Biography = request.Biography,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                ImageUrl = request.ImageUrl,
                Address = request.Address,
                PersonalLinks = request.PersonalLinks
            };
            _dbContext.Doctors.Add(doctor);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(doctor.Id,_localizationService.GetLocalizedString(LocalizationKeys.Doctors.Created));
        }
    }
}
