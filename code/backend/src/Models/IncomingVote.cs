using System.ComponentModel.DataAnnotations;
using backend.src.Attributes;

namespace backend.src.Models;

public record Ballots : ListModel<Ballot>
{
    [Required, CredencialCivica]
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

    public int? ListNumber { get; init; }

    public string[] ListCandidateNames { get; init; } = [];

    public SimplifiedParty? Party { get; init; }

    public bool? IsYes { get; init; }
}
