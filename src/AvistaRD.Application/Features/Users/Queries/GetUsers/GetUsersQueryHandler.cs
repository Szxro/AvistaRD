using AvistaRD.Application.Common.Abstractions;
using AvistaRD.Application.Common.Models;
using AvistaRD.Application.Contracts;
using AvistaRD.SharedKernel;

namespace AvistaRD.Application.Features.Users.Queries.GetUsers;

public sealed class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, PagedResult<UserResponse>>
{
    private readonly IUserAccountService _userAccountService;

    public GetUsersQueryHandler(IUserAccountService userAccountService)
    {
        _userAccountService = userAccountService;
    }

    public async Task<Result<PagedResult<UserResponse>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        PagedResult<UserResponse> users = await _userAccountService.GetPagedAsync(
            request.Status,
            request.Search,
            request.Page,
            request.PageSize,
            cancellationToken);

        return Result<PagedResult<UserResponse>>.Success(users);
    }
}
