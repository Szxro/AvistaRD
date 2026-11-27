using AvistaRD.Api.Common;
using AvistaRD.Api.Common.Response;
using AvistaRD.Application.Features.Users;
using AvistaRD.Application.Features.Users.Queries.GetCurrentUser;
using AvistaRD.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AvistaRD.Api.Controllers;

[Route("api/users")]
[Authorize]
public class UsersController : BaseController
{
    public UsersController(ISender sender) : base(sender) { }

    // The frontend reads the role from here to decide which screens to show
    [HttpGet("me")]
    [ProducesResponseType<SuccessResponse<UserResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        Result<UserResponse> result = await Sender.Send(new GetCurrentUserQuery(), cancellationToken);

        return result.IsSuccess ? Success(result) : Problem(result);
    }
}
