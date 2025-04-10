using FluentValidation;
using MediatR;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Domain.ValueObjects;
using WebApi.Infrastructure.Persistence;

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
        public UpdateDoctorValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than zero.");
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");
            RuleFor(x => x.Speciality)
                .NotEmpty().WithMessage("Speciality is required.")
                .MaximumLength(50).WithMessage("Speciality must not exceed 50 characters.");
            RuleFor(x => x.SummaryInfo)
                .NotEmpty().WithMessage("Summary info is required.")
                .MaximumLength(500).WithMessage("Summary info must not exceed 500 characters.");
            RuleFor(x => x.Biography)
                .NotEmpty().WithMessage("Biography is required.")
                .MaximumLength(2000).WithMessage("Biography must not exceed 2000 characters.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");
            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("Image URL is required.")
                .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute)).WithMessage("Invalid image URL format.");
        }

        public sealed class UpdateDoctorHandler : IRequestHandler<UpdateDoctorCommand, Result<int>>
        {
            private readonly ApplicationDbContext _dbContext;
            public UpdateDoctorHandler(ApplicationDbContext dbContext)
            {
                _dbContext = dbContext;
            }
            public async Task<Result<int>> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
            {
                var doctor = await _dbContext.Doctors.FindAsync(new object[] { request.Id }, cancellationToken);
                if (doctor == null)
                {
                    return Result<int>.Failure("Doctor not found.");
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
                return Result<int>.Success(doctor.Id);
            }
        }
    }
}
