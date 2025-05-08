using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using backend.src.Validators;

namespace backend.src.Models;

public record IncomingVotes
{
    [Required] public required List<IncomingVote> Votes { get; init; }
    [Required, CredencialCivicaValidator] public required string VoterCredencialCivica;
}

public enum VoteType
{
    List,
    Boolean,
    Count,
}

public record IncomingVote
{
    [Required]
    public required VoteType Type { get; init; }

    [Required]
    public required Guid ElectionId { get; init; }

    [Description($"Specify if vote type is {nameof(VoteType.List)}.")]
    public int? ListId { get; init; }

    [Description($"Specify if vote type is {nameof(VoteType.Boolean)}")]
    public bool? IsYes { get; init; }
}
