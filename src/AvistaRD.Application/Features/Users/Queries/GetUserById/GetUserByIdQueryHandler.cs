using AvistaRD.Application.Common.Abstractions;
using AvistaRD.Application.Contracts;
using AvistaRD.SharedKernel;

namespace AvistaRD.Application.Features.Users.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserResponse>
{
    private readonly IUserAccountService _userAccountService;

    public GetUserByIdQueryHandler(IUserAccountService userAccountService)
    {
        _userAccountService = userAccountService;
    }

    public async Task<Result<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        UserResponse? user = await _userAccountService.GetByIdAsync(request.UserId, cancellationToken);

        return user is null
            ? Result<UserResponse>.Failure(UserErrors.NotFound)
            : Result<UserResponse>.Success(user);
    }
}
