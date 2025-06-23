using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using backend.src.Attributes;

namespace backend.src.Models;

public record Ballots : ListModel<Ballot>
{
    [Required, CredencialCivicaAttribute]
    public required string VoterCredencialCivica;
}

public enum BallotType
{
    List,
    Boolean,
    Count,
}

public record Ballot
{
    [Required]
    public required Ulid ElectionId { get; init; }

    [Required]
    public required Ulid BallotId { get; init; }

    [Description($"Specify if vote type is {nameof(BallotType.List)}.")]
    public int? ListNumber { get; init; }

    [Description($"Specify if vote type is {nameof(BallotType.Boolean)}.")]
    public bool? IsYes { get; init; }
}
