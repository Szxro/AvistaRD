using AvistaRD.SharedKernel;
using AvistaRD.SharedKernel.Common.Enums;
using FluentValidation.Results;

namespace AvistaRD.Application.Common.Errors;


public class ValidationError : Error
{
    public Dictionary<string, List<ErrorResponse>> Errors { get; }

    public ValidationError(ValidationFailure[] failures)
        : base("Validation.Error",
            "One or more validation errors occurred.",
            ErrorType.Validation)
    {
        Errors = GroupFailuresByPropertyName(failures);
    }

    private Dictionary<string, List<ErrorResponse>> GroupFailuresByPropertyName(ValidationFailure[] failures)
    {
        return failures
            .GroupBy(property => property.PropertyName)
            .ToDictionary(
                property => property.Key,
                property => 
                    property.Select(x => new ErrorResponse(x.ErrorCode, x.ErrorMessage)).ToList());
    }
}

public record ErrorResponse(string ErrorCode, string ErrorMessage); 