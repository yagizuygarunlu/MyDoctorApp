using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using WebApi.Common.Results;
using WebApi.Infrastructure.Persistence;
using WebApi.Infrastructure.Services;

namespace WebApi.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService;
        private readonly PasswordService _passwordService;

        public LoginCommandHandler(ApplicationDbContext context, AuthService authService, PasswordService passwordService)
        {
            _context = context;
            _authService = authService;
            _passwordService = passwordService;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user == null || !_passwordService.VerifyPassword(user.PasswordHash, request.Password))
                throw new UnauthorizedAccessException("Invalid credentials.");

            var token = _authService.GenerateJwtToken(user);

            return new LoginResponse(token);
        }
    }
}
