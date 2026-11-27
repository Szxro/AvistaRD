using FluentValidation;

namespace AvistaRD.Application.Features.Users.Queries.GetUserById;

public sealed class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdQueryValidator()
    {
        RuleFor(query => query.UserId)
            .NotEmpty()
            .MaximumLength(450);
    }
}
