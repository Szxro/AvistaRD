using AvistaRD.Application.Common.Abstractions;
using AvistaRD.Application.Common.Models;
using AvistaRD.Domain.Users;

namespace AvistaRD.Application.Features.Users.Queries.GetUsers;

// Status null lists every account; Search matches the email or the full name
public sealed record GetUsersQuery(UserStatus? Status, string? Search, int Page, int PageSize) : IQuery<PagedResult<UserResponse>>;
