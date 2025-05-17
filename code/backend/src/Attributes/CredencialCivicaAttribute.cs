using System.ComponentModel.DataAnnotations;

namespace backend.src.Attributes;

public class CredencialCivicaAttribute : RegularExpressionAttribute
{
    public CredencialCivicaAttribute()
        : base(@"^[A-Z]{3}\d+$") { }
}
