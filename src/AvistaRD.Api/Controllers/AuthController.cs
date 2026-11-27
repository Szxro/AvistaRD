using System.Security.Claims;
using AvistaRD.Api.Common;
using AvistaRD.Api.Common.Response;
using AvistaRD.Application.Features.Auth.Commands.Login;
using AvistaRD.Application.Features.Auth.Commands.RefreshToken;
using AvistaRD.Application.Features.Auth.Commands.Register;
using AvistaRD.Application.Features.Users;
using AvistaRD.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AvistaRD.Api.Controllers;

[Route("api/auth")]
[AllowAnonymous]
public class AuthController : BaseController
{
    public AuthController(ISender sender) : base(sender) { }

    // Ciudadano (the default role) is active right away; Validador and Guardaparques start pending
    [HttpPost("register")]
    [ProducesResponseType<SuccessResponse<UserResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
    {
        Result<UserResponse> result = await Sender.Send(command, cancellationToken);

        return result.IsSuccess ? Success(result) : Problem(result);
    }

    // 401 for wrong credentials; 403 for locked, pending or deactivated accounts (the title says which)
    [HttpPost("login")]
    [ProducesResponseType<AccessTokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        Result<ClaimsPrincipal> result = await Sender.Send(command, cancellationToken);

        return result.IsSuccess ? IssueTokens(result.Value) : Problem(result);
    }

    [HttpPost("refresh")]
    [ProducesResponseType<AccessTokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IResult> Refresh([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        Result<ClaimsPrincipal> result = await Sender.Send(command, cancellationToken);

        return result.IsSuccess ? IssueTokens(result.Value) : Problem(result);
    }

    // The Identity bearer token handler writes the AccessTokenResponse (access and refresh tokens) as the body
    private static IResult IssueTokens(ClaimsPrincipal principal)
    {
        return Results.SignIn(principal, authenticationScheme: IdentityConstants.BearerScheme);
    }
}
