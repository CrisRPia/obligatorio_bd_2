using System.ComponentModel.DataAnnotations;

namespace backend.src.Validators;

[AttributeUsage(
    AttributeTargets.Property
        | AttributeTargets.Field
        | AttributeTargets.Parameter,
    AllowMultiple = false
)]
class UruguayanIdValidator : ValidationAttribute
{
    private static readonly int[] Multipliers = [2, 9, 8, 7, 6, 3, 4];
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
        {
            return ValidationResult.Success!;
        }

        if (value is not int)
            return BadResult(validationContext);

        var checkedValue = (int)value;
        var idDigits = checkedValue
            .ToString()
            .ToCharArray()
            .Select(x => (int)char.GetNumericValue(x))
            .ToArray();

        if (idDigits.Length != 8)
            return BadResult(validationContext);

        var verifierDigit = Multipliers.Zip(idDigits, (a, b) => a * b).Sum();

        if (verifierDigit == idDigits.Last())
        {
            return ValidationResult.Success!;
        }

        return BadResult(validationContext);
    }
}
