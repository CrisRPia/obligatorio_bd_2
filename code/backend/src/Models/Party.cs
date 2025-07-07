using System.ComponentModel.DataAnnotations;

namespace backend.src.Models;

public record Party
{
    [Required]
    public required string HeadquartersAddress { get; init; }

    [Required]
    public required string Name { get; init; }

    [Required]
    public required Ulid PartyId { get; init; }

    [Required]
    public required IReadOnlyList<BaseCitizen> Citizens { get; init; }
}

public record SimplifiedParty {
    [Required]
    public required string HeadquartersAddress { get; init; }

    [Required]
    public required Ulid PartyId { get; init; }

    [Required]
    public required string Name { get; init; }
}
