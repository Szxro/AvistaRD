using AvistaRD.Application.Common.Errors;
using AvistaRD.Application.Common.Models;
using AvistaRD.Application.Contracts;
using AvistaRD.Application.Features.Users;
using AvistaRD.Domain.Users;
using AvistaRD.Infrastructure.Common.Attributes;
using AvistaRD.Infrastructure.Common.Enums;
using AvistaRD.Infrastructure.Identity;
using AvistaRD.Infrastructure.Persistence;
using AvistaRD.SharedKernel;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AvistaRD.Infrastructure.Services;

[Inject(ServiceKind.Service, ServiceLifetime.Scoped)]
public class UserAccountService : IUserAccountService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserAccountService(
        AppDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<PagedResult<UserResponse>> GetPagedAsync(
        UserStatus? status,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<ApplicationUser> users = _context.Users.AsNoTracking();

        if (status is { } statusFilter)
        {
            users = users.Where(user => user.Status == statusFilter);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            string term = search.Trim();

            users = users.Where(user => user.Email!.Contains(term) || user.FullName.Contains(term));
        }

        int totalCount = await users.CountAsync(cancellationToken);

        // Newest first; the id breaks ties so the pages are stable
        List<UserResponse> items = await ProjectToResponse(users
                .OrderByDescending(user => user.CreatedAt)
                .ThenBy(user => user.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize))
            .ToListAsync(cancellationToken);

        return new PagedResult<UserResponse>(items, page, pageSize, totalCount);
    }

    public async Task<UserResponse?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await ProjectToResponse(_context.Users.AsNoTracking().Where(user => user.Id == userId))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Result<UserResponse>> CreateAsync(
        string email,
        string password,
        string fullName,
        string role,
        UserStatus status,
        CancellationToken cancellationToken = default)
    {
        // Same convention as Identity's own registration: the email is also the user name
        ApplicationUser user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            Status = status
        };

        IdentityResult createResult = await _userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
        {
            return Result<UserResponse>.Failure(ToError(createResult));
        }

        // The roles are seeded by a migration, so a failure here is a bug. Throwing makes the
        // transaction pipeline roll back the new user as well.
        EnsureSucceeded(await _userManager.AddToRoleAsync(user, role));

        return Result<UserResponse>.Success(await GetRequiredResponseAsync(user.Id, cancellationToken));
    }

    public async Task<UserResponse> ChangeStatusAsync(string userId, UserStatus status, CancellationToken cancellationToken = default)
    {
        ApplicationUser user = await FindRequiredAsync(userId);

        user.Status = status;

        // A new security stamp invalidates the refresh tokens and, through SessionValidationMiddleware,
        // the access tokens already issued. Not needed when activating: the account has no valid tokens.
        IdentityResult result = status == UserStatus.Active
            ? await _userManager.UpdateAsync(user)
            : await _userManager.UpdateSecurityStampAsync(user);

        EnsureSucceeded(result);

        return await GetRequiredResponseAsync(userId, cancellationToken);
    }

    public async Task<UserResponse> UpdateAsync(string userId, string fullName, string role, CancellationToken cancellationToken = default)
    {
        ApplicationUser user = await FindRequiredAsync(userId);

        user.FullName = fullName;

        IList<string> currentRoles = await _userManager.GetRolesAsync(user);

        bool roleChanged = currentRoles.Count != 1 || currentRoles[0] != role;

        if (!roleChanged)
        {
            EnsureSucceeded(await _userManager.UpdateAsync(user));

            return await GetRequiredResponseAsync(userId, cancellationToken);
        }

        // One role per account: replace whatever the account had
        if (currentRoles.Count > 0)
        {
            EnsureSucceeded(await _userManager.RemoveFromRolesAsync(user, currentRoles));
        }

        EnsureSucceeded(await _userManager.AddToRoleAsync(user, role));

        // The issued tokens carry the old role: a new security stamp makes the user sign in again
        EnsureSucceeded(await _userManager.UpdateSecurityStampAsync(user));

        return await GetRequiredResponseAsync(userId, cancellationToken);
    }

    private IQueryable<UserResponse> ProjectToResponse(IQueryable<ApplicationUser> users)
    {
        return users.Select(user => new UserResponse(
            user.Id,
            user.Email ?? string.Empty,
            user.FullName,
            _context.UserRoles
                .Where(userRole => userRole.UserId == user.Id)
                .Join(_context.Roles,
                    userRole => userRole.RoleId,
                    role => role.Id,
                    (userRole, role) => role.Name)
                .OrderBy(roleName => roleName)
                .FirstOrDefault(),
            user.Status,
            user.CreatedAt));
    }

    private async Task<UserResponse> GetRequiredResponseAsync(string userId, CancellationToken cancellationToken)
    {
        return await GetByIdAsync(userId, cancellationToken)
               ?? throw new InvalidOperationException($"The user account {userId} doesn't exist.");
    }

    private async Task<ApplicationUser> FindRequiredAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId)
               ?? throw new InvalidOperationException($"The user account {userId} doesn't exist.");
    }

    private static Error ToError(IdentityResult result)
    {
        if (result.Errors.Any(error => error.Code is nameof(IdentityErrorDescriber.DuplicateUserName)
                                                  or nameof(IdentityErrorDescriber.DuplicateEmail)))
        {
            return UserErrors.EmailAlreadyRegistered;
        }

        // Password policy and email format errors, grouped by field like the FluentValidation ones
        ValidationFailure[] failures = result.Errors
            .Select(error => new ValidationFailure(
                error.Code.StartsWith("Password", StringComparison.Ordinal) ? "Password" : "Email",
                error.Description)
            {
                ErrorCode = error.Code
            })
            .ToArray();

        return new ValidationError(failures);
    }

    private static void EnsureSucceeded(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            string errors = string.Join(" ", result.Errors.Select(error => error.Description));

            throw new InvalidOperationException($"The user account couldn't be updated: {errors}");
        }
    }
}
