using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace backend.src.Validators;

public partial class CredencialCivicaValidator
{
    [GeneratedRegex(@"^[A-Z]{3}\d+$")]
    private static partial Regex CredencialCivicaRegex();
}

public partial class CredencialCivicaValidator : ValidationAttribute
{
    public CredencialCivicaValidator()
    {
        ErrorMessage =
            "Invalid credencial cívica format. It should be 3 uppercase letters followed by numbers.";
    }

    private ValidationResult ValidationError(ValidationContext context) =>
        new(ErrorMessage, [context.MemberName!]);

    protected override ValidationResult IsValid(
        object? value,
        ValidationContext validationContext
    )
    {
        if (value is null)
            return ValidationResult.Success!;

        if (value is not string credencial)
            return ValidationError(validationContext);

        if (!CredencialCivicaRegex().IsMatch(credencial))
            return ValidationError(validationContext);

        return ValidationResult.Success!;
    }
}
