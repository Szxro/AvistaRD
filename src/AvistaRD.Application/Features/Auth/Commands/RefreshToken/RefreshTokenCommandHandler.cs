using System.Security.Claims;
using AvistaRD.Application.Common.Abstractions;
using AvistaRD.Application.Contracts;
using AvistaRD.SharedKernel;

namespace AvistaRD.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, ClaimsPrincipal>
{
    private readonly IAuthService _authService;

    public RefreshTokenCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<ClaimsPrincipal>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await _authService.RefreshAsync(request.RefreshToken, cancellationToken);
    }
}
