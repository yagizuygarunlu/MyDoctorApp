
using MediatR;

namespace WebApi.Features.Auth.Commands.Login
{
    public record LoginCommand(
        string Email,
        string Password
    ) : IRequest<LoginResponse>;

}
