using MediatR;
using WebApi.Common.Results;
using WebApi.Infrastructure.Persistence;
using FluentValidation;
using WebApi.Common.Localization;

namespace WebApi.Features.Doctors.Commands.Reactivate
{
    public record ReactivateDoctorCommand(
        int Id
    ) : IRequest<Result<int>>;

    public class ReactivateDoctorValidator : AbstractValidator<ReactivateDoctorCommand>
    {
        public ReactivateDoctorValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Doctors.InvalidId));
        }
    }

    public sealed class ReactivateDoctorHandler : IRequestHandler<ReactivateDoctorCommand, Result<int>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILocalizationService _localizationService;

        public ReactivateDoctorHandler(ApplicationDbContext dbContext, ILocalizationService localizationService)
        {
            _dbContext = dbContext;
            _localizationService = localizationService;
        }

        public async Task<Result<int>> Handle(ReactivateDoctorCommand request, CancellationToken cancellationToken)
        {
            var doctor = await _dbContext.Doctors.FindAsync(request.Id);
            if (doctor == null)
            {
                return Result<int>.Failure(_localizationService.GetLocalizedString(LocalizationKeys.Doctors.NotFound));
            }

            doctor.IsActive = true;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(request.Id, _localizationService.GetLocalizedString(LocalizationKeys.Doctors.Reactivated));
        }
    }
}
