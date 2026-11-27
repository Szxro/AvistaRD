using FluentValidation;

namespace AvistaRD.Application.Features.Users.Queries.GetUsers;

public sealed class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public const int MaxPageSize = 100;

    public GetUsersQueryValidator()
    {
        RuleFor(query => query.Status)
            .IsInEnum();

        RuleFor(query => query.Search)
            .MaximumLength(100);

        RuleFor(query => query.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, MaxPageSize);
    }
}
