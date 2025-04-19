using MediatR;
using WebApi.Features.Auth.Commands.Login;

namespace WebApi.Features.Auth.Commands
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/auth").WithTags("Auth");
            group.MapPost("/login", async (IMediator _mediator, LoginCommand request, CancellationToken cancellationToken) =>
            {
                var response = await _mediator.Send(request, cancellationToken);
                return Results.Ok(response);
            });


            //group.MapPost("/register", async (RegisterRequest request, IAuthService authService) =>
            //{
            //    var response = await authService.RegisterAsync(request);
            //    return Results.Ok(response);
            //})
            //.WithName("Register")
            //.Produces(StatusCodes.Status201Created)
            //.Produces(StatusCodes.Status400BadRequest);
        }
    }
}
