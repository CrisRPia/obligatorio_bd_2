using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace backend.src.Models;

public record CandidateList
{
    [Required]
    [MinLength(1)]
    [Description(
        "The ordered list of candidates on this voting list. The first candidate is the main candidate."
    )]
    public required IEnumerable<Candidate> Candidates { get; init; }

    [Required]
    public required Party Party { get; init; }

    [Required]
    public required int ListNumber { get; init; }
}
