using AvistaRD.Application.Common.Models;
using AvistaRD.Application.Features.Users;
using AvistaRD.Domain.Users;
using AvistaRD.SharedKernel;

namespace AvistaRD.Application.Contracts;

// Data access for user accounts (ASP.NET Core Identity). The business rules live in the handlers.
public interface IUserAccountService
{
    Task<PagedResult<UserResponse>> GetPagedAsync(
        UserStatus? status,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<UserResponse?> GetByIdAsync(string userId, CancellationToken cancellationToken = default);

    // Fails with a validation error (password policy) or a conflict (email already registered)
    Task<Result<UserResponse>> CreateAsync(
        string email,
        string password,
        string fullName,
        string role,
        UserStatus status,
        CancellationToken cancellationToken = default);

    // Leaving Active rotates the security stamp, so the account's tokens stop working
    Task<UserResponse> ChangeStatusAsync(string userId, UserStatus status, CancellationToken cancellationToken = default);

    // Changing the role rotates the security stamp, because the issued tokens carry the old role
    Task<UserResponse> UpdateAsync(string userId, string fullName, string role, CancellationToken cancellationToken = default);
}
