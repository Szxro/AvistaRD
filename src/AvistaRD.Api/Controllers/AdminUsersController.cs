using AvistaRD.Api.Common;
using AvistaRD.Api.Common.Response;
using AvistaRD.Api.Contracts.Users;
using AvistaRD.Application.Common.Models;
using AvistaRD.Application.Features.Users;
using AvistaRD.Application.Features.Users.Commands.ApproveUser;
using AvistaRD.Application.Features.Users.Commands.DeactivateUser;
using AvistaRD.Application.Features.Users.Commands.ReactivateUser;
using AvistaRD.Application.Features.Users.Commands.UpdateUser;
using AvistaRD.Application.Features.Users.Queries.GetUserById;
using AvistaRD.Application.Features.Users.Queries.GetUsers;
using AvistaRD.Domain.Users;
using AvistaRD.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AvistaRD.Api.Controllers;

// Account management: only Administrador. Other roles get 403 and anonymous requests 401.
[Route("api/admin/users")]
[Authorize(Roles = Roles.Administrator)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public class AdminUsersController : BaseController
{
    public AdminUsersController(ISender sender) : base(sender) { }

    // status: Pending, Active or Deactivated (omit it to list every account)
    [HttpGet]
    [ProducesResponseType<SuccessResponse<PagedResult<UserResponse>>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetUsers(
        [FromQuery] UserStatus? status,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        Result<PagedResult<UserResponse>> result = await Sender.Send(
            new GetUsersQuery(status, search, page, pageSize),
            cancellationToken);

        return result.IsSuccess ? Success(result) : Problem(result);
    }

    [HttpGet("{userId}")]
    [ProducesResponseType<SuccessResponse<UserResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetUser(string userId, CancellationToken cancellationToken)
    {
        Result<UserResponse> result = await Sender.Send(new GetUserByIdQuery(userId), cancellationToken);

        return result.IsSuccess ? Success(result) : Problem(result);
    }

    // Changing the role closes the user's sessions (their tokens carry the old role)
    [HttpPut("{userId}")]
    [ProducesResponseType<SuccessResponse<UserResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IResult> UpdateUser(string userId, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        Result<UserResponse> result = await Sender.Send(
            new UpdateUserCommand(userId, request.FullName, request.Role),
            cancellationToken);

        return result.IsSuccess ? Success(result) : Problem(result);
    }

    // Pending -> Active: the user can sign in
    [HttpPost("{userId}/approve")]
    [ProducesResponseType<SuccessResponse<UserResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IResult> ApproveUser(string userId, CancellationToken cancellationToken)
    {
        Result<UserResponse> result = await Sender.Send(new ApproveUserCommand(userId), cancellationToken);

        return result.IsSuccess ? Success(result) : Problem(result);
    }

    // Active or Pending -> Deactivated: the user's tokens stop working; their records are kept
    [HttpPost("{userId}/deactivate")]
    [ProducesResponseType<SuccessResponse<UserResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IResult> DeactivateUser(string userId, CancellationToken cancellationToken)
    {
        Result<UserResponse> result = await Sender.Send(new DeactivateUserCommand(userId), cancellationToken);

        return result.IsSuccess ? Success(result) : Problem(result);
    }

    // Deactivated -> Active
    [HttpPost("{userId}/reactivate")]
    [ProducesResponseType<SuccessResponse<UserResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IResult> ReactivateUser(string userId, CancellationToken cancellationToken)
    {
        Result<UserResponse> result = await Sender.Send(new ReactivateUserCommand(userId), cancellationToken);

        return result.IsSuccess ? Success(result) : Problem(result);
    }
}
