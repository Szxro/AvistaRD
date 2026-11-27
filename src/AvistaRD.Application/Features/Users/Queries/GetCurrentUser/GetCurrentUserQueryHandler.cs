using AvistaRD.Application.Common.Abstractions;
using AvistaRD.Application.Contracts;
using AvistaRD.Application.Features.Auth;
using AvistaRD.SharedKernel;

namespace AvistaRD.Application.Features.Users.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, UserResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserAccountService _userAccountService;

    public GetCurrentUserQueryHandler(
        ICurrentUserService currentUserService,
        IUserAccountService userAccountService)
    {
        _currentUserService = currentUserService;
        _userAccountService = userAccountService;
    }

    public async Task<Result<UserResponse>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is not { } userId)
        {
            return Result<UserResponse>.Failure(AuthErrors.NotAuthenticated);
        }

        UserResponse? user = await _userAccountService.GetByIdAsync(userId, cancellationToken);

        return user is null
            ? Result<UserResponse>.Failure(UserErrors.NotFound)
            : Result<UserResponse>.Success(user);
    }
}
