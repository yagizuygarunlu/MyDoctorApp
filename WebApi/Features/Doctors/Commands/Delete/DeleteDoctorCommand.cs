using FluentValidation;
using MediatR;
using WebApi.Common.Localization;
using WebApi.Common.Results;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.Doctors.Commands.Delete
{
    public record DeleteDoctorCommand(
            int Id
        ) : IRequest<Result<int>>;

    public class DeleteDoctorValidator : AbstractValidator<DeleteDoctorCommand>
    {
        public DeleteDoctorValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage(localizationService.GetLocalizedString(LocalizationKeys.Doctors.NotFound));
        }
    }

    public sealed class DeleteDoctorHandler : IRequestHandler<DeleteDoctorCommand, Result<int>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILocalizationService _localizationService;

        public DeleteDoctorHandler(ApplicationDbContext dbContext, ILocalizationService localizationService)
        {
            _dbContext = dbContext;
            _localizationService = localizationService;
        }

        public async Task<Result<int>> Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
        {
            var doctor = await _dbContext.Doctors.FindAsync(request.Id);
            if (doctor == null)
            {
                return Result<int>.Failure(_localizationService.GetLocalizedString(LocalizationKeys.Doctors.NotFound));
            }

            doctor.IsActive = false;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(request.Id, _localizationService.GetLocalizedString(LocalizationKeys.Doctors.Deleted));
        }
    }
}
