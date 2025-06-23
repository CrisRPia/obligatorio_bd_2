namespace backend.src.Services;

public class UruguayanIdVerifier
{
    private static readonly int[] Multipliers = [2, 9, 8, 7, 6, 3, 4];

    public static int? GetValidationDigit(int value)
    {
        var idDigits = value
            .ToString()
            .ToCharArray()
            .Select(x => (int)char.GetNumericValue(x))
            .ToArray();

        if (idDigits.Length < 1 || idDigits.Length > 7)
            return null;

        return CalculateDigit(idDigits);
    }

    private static int CalculateDigit(int[] idDigits)
    {
        var normalizedDigits = new int[7];
        int offset = 7 - idDigits.Length;

        for (int i = 0; i < offset; i++)
            normalizedDigits[i] = 0;

        for (int i = 0; i < idDigits.Length; i++)
            normalizedDigits[i + offset] = idDigits[i];

        int sum = 0;

        for (int i = 0; i < 7; i++)
        {
            sum += normalizedDigits[i] * Multipliers[i];
        }

        return (10 - (sum % 10)) % 10;
    }
}
