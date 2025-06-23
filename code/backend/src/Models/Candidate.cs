using System.ComponentModel.DataAnnotations;

namespace backend.src.Models;

public record Candidate
{
    [Required]
    public required Ulid CandidateId { get; init; }

    [Required]
    public required string Name { get; init; }

    [Required]
    public required string Surname { get; init; }

    [Required]
    public required int Position { get; init; }
}
