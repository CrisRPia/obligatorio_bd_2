using System.ComponentModel.DataAnnotations;

namespace backend.src.Models;

/// <summary>
/// Defines the different types of electoral events held in Uruguay.
/// </summary>
public enum ElectionType
{
    /// <summary>
    /// Election for the President and Vice-President of the Republic.
    /// </summary>
    Presidential,

    /// <summary>
    /// A popular vote to approve or repeal a specific law passed by the legislative power.
    /// </summary>
    Referendum,

    /// <summary>
    /// A popular vote to approve or reject a proposed amendment to the Constitution.
    /// </summary>
    Plebiscite,

    /// <summary>
    /// Election for local authorities (Mayors/Alcaldes and Municipal Councils) within a department.
    /// Note: This election is typically held concurrently with Departmental Elections (Intendente and Junta Departamental).
    /// </summary>
    MunicipalElection,

    /// <summary>
    /// A second round of voting in a Presidential Election if no candidate achieves an absolute majority in the first round. Also known as a Runoff.
    /// </summary>
    Runoff,
}

public enum ElectionState
{
    NotStarted,
    Open,
    Closed,
}

public record Election
{
    [Required]
    public required ElectionType Type { get; init; }

    [Required]
    public required DateOnly Date { get; init; }

    [Required]
    public required IReadOnlyList<Ballot> AllowedBallots { get; init; }

    [Required]
    public required Ulid ElectionId { get; init; }

    public Ulid? DepartmentId { get; init; }

    [Required]
    public required ElectionState State { get; init; }
}

public record ElectionResult
{
    [Required]
    public required ElectionType Type { get; init; }

    public IEnumerable<VoteResult<BooleanVote>>? BooleanResult { get; init; }
    public IEnumerable<VoteResult<Ballot>>? ListBasedResult { get; init; }

    [Required]
    public int TotalVotes { get; init; }
}

public record BooleanVote
{
    [Required]
    public required bool Yes { get; init; }

    [Required]
    [MinLength(3), MaxLength(3)]
    public required string ColorHex { get; init; }
}

public record VoteResult<T>
{
    [Required]
    public required int VoteCount { get; init; }

    [Required]
    public required T Vote { get; init; }
}
