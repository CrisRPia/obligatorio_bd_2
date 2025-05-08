using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace backend.src.Models;

public record ElectionList
{
    [
        Required,
        MinLength(1),
        Description(
            "The ordered list of candidates on this voting list. The first candidate is the main candidate."
        )
    ]
    public required List<Candidate> Candidates { get; init; }

    [Required]
    public required Party Party { get; init; }

    [Required]
    public required int ListNumber { get; init; }
}
