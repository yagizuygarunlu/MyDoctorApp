using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Common.Results;
using WebApi.Infrastructure.Persistence;
using WebApi.Infrastructure.Services;

namespace WebApi.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly AuthService _authService;
        private readonly PasswordService _passwordService;
        private readonly ILocalizationService _localizationService;

        public LoginCommandHandler(
            IApplicationDbContext context,
            AuthService authService,
            PasswordService passwordService,
            ILocalizationService localizationService)
        {
            _context = context;
            _authService = authService;
            _passwordService = passwordService;
            _localizationService = localizationService;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user == null || !_passwordService.VerifyPassword(user.PasswordHash, request.Password))
                throw new UnauthorizedAccessException(_localizationService.GetLocalizedString(LocalizationKeys.Auth.InvalidCredentials));

            var token = _authService.GenerateJwtToken(user);

            return new LoginResponse(token);
        }
    }
}
