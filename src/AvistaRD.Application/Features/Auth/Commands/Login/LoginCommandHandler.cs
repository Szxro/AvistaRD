using System.Security.Claims;
using AvistaRD.Application.Common.Abstractions;
using AvistaRD.Application.Contracts;
using AvistaRD.SharedKernel;

namespace AvistaRD.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, ClaimsPrincipal>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<ClaimsPrincipal>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _authService.AuthenticateAsync(request.Email.Trim(), request.Password, cancellationToken);
    }
}
