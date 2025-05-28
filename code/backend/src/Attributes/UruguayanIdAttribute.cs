using System.ComponentModel.DataAnnotations;
using backend.src.Services;

namespace backend.src.Attributes;

[AttributeUsage(
    AttributeTargets.Property
        | AttributeTargets.Field
        | AttributeTargets.Parameter,
    AllowMultiple = false
)]
class UruguayanIdAttribute : ValidationAttribute
{
    private const string DefaultErrorMessage =
        "Invalid Uruguayan Identity Document number.";

    private static ValidationResult BadResult(
        ValidationContext validationContext
    ) => new(DefaultErrorMessage, [validationContext.MemberName!]);

    protected override ValidationResult IsValid(
        object? value,
        ValidationContext validationContext
    )
    {
        if (value == null)
            return ValidationResult.Success!;

        if (value is not int checkedValue)
            return BadResult(validationContext);

        var verifierDigit = UruguayanIdVerifier.GetValidationDigit(
            checkedValue
        );

        if (verifierDigit != checkedValue % 10)
            return BadResult(validationContext);

        return ValidationResult.Success!;
    }
}
