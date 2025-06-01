using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Localization;
using WebApi.Domain.Entities;
using WebApi.Features.Auth.Commands.Login;
using WebApi.Infrastructure.Services;

namespace WebApi.Tests.Features.Auth.Commands.Login
{
    public class LoginCommandHandlerTests
    {
        private readonly IApplicationDbContext _context;
        private readonly AuthService _authService;
        private readonly PasswordService _passwordService;
        private readonly ILocalizationService _localizationService;
        private readonly LoginCommandHandler _handler;

        public LoginCommandHandlerTests()
        {
            _context = Substitute.For<IApplicationDbContext>();
            _localizationService = Substitute.For<ILocalizationService>();

            // Create real instances of services since they're hard to mock
            var configuration = Substitute.For<IConfiguration>();
            configuration["Jwt:Key"].Returns("this-is-a-very-long-secret-key-for-jwt-token-generation-that-is-at-least-256-bits");
            configuration["Jwt:Issuer"].Returns("TestIssuer");
            configuration["Jwt:Audience"].Returns("TestAudience");
            
            _authService = new AuthService(configuration);
            _passwordService = new PasswordService();

            _localizationService.GetLocalizedString(LocalizationKeys.Auth.InvalidCredentials)
                .Returns("Invalid credentials");

            _handler = new LoginCommandHandler(_context, _authService, _passwordService, _localizationService);
        }

        [Fact]
        public void Command_ShouldContainCorrectProperties()
        {
            // Arrange & Act
            var command = new LoginCommand("test@example.com", "password123");

            // Assert
            command.Email.ShouldBe("test@example.com");
            command.Password.ShouldBe("password123");
        }

        [Fact]
        public void Response_ShouldContainCorrectProperties()
        {
            // Arrange & Act
            var response = new LoginResponse("jwt-token-123");

            // Assert
            response.Token.ShouldBe("jwt-token-123");
        }

        [Fact]
        public void Handler_ShouldHaveCorrectConstructor()
        {
            // Arrange & Act
            var handler = new LoginCommandHandler(_context, _authService, _passwordService, _localizationService);

            // Assert
            handler.ShouldNotBeNull();
        }

        [Fact]
        public void AuthService_ShouldGenerateValidToken()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                FullName = "Test User",
                Role = UserRole.Admin
            };

            // Act
            var token = _authService.GenerateJwtToken(user);

            // Assert
            token.ShouldNotBeNullOrEmpty();
            token.Split('.').Length.ShouldBe(3); // JWT has 3 parts separated by dots
        }

        [Fact]
        public void PasswordService_ShouldHashAndVerifyPasswords()
        {
            // Arrange
            var password = "testPassword123";

            // Act
            var hashedPassword = _passwordService.HashPassword(password);
            var isValid = _passwordService.VerifyPassword(hashedPassword, password);
            var isInvalid = _passwordService.VerifyPassword(hashedPassword, "wrongPassword");

            // Assert
            hashedPassword.ShouldNotBeNullOrEmpty();
            hashedPassword.ShouldNotBe(password); // Should be hashed, not plain text
            isValid.ShouldBeTrue();
            isInvalid.ShouldBeFalse();
        }

        [Fact]
        public void Handler_ShouldImplementCorrectInterface()
        {
            // Arrange & Act
            var handler = new LoginCommandHandler(_context, _authService, _passwordService, _localizationService);

            // Assert
            handler.ShouldBeAssignableTo<MediatR.IRequestHandler<LoginCommand, LoginResponse>>();
        }

        [Fact]
        public void Command_ShouldImplementCorrectInterface()
        {
            // Arrange & Act
            var command = new LoginCommand("test@example.com", "password123");

            // Assert
            command.ShouldBeAssignableTo<MediatR.IRequest<LoginResponse>>();
        }
    }
} 