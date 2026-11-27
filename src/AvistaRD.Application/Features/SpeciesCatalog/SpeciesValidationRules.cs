using FluentValidation;

namespace AvistaRD.Application.Features.SpeciesCatalog;

public static class SpeciesValidationRules
{
    public const int MaxNameLength = 150;

    public static IRuleBuilderOptions<T, string> ValidSpeciesName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("{PropertyName} can't be empty")
            .MaximumLength(MaxNameLength).WithMessage("{PropertyName} can't exceed {MaxLength} characters");
    }
}
