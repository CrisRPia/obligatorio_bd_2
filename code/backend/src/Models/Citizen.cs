using System.ComponentModel.DataAnnotations;
using backend.src.Attributes;

namespace backend.src.Models;

public record BaseCitizen
{
    [Required, CredencialCivicaAttribute]
    public required string CredencialCivica { get; init; }

    [Required, UruguayanIdAttribute]
    public required int UruguayanId { get; init; }
}

public record PartialCitizen : BaseCitizen
{
    public DateOnly? BirthDate { get; init; }
    public string? Name { get; init; }
    public string? Surname { get; init; }
}

public record FullCitizen : BaseCitizen
{
    [Required]
    public required DateOnly BirthDate { get; init; }

    [Required]
    public required string Name { get; init; }

    [Required]
    public required string Surname { get; init; }
}
