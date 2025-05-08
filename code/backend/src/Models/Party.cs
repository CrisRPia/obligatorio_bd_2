using System.ComponentModel.DataAnnotations;

namespace backend.src.Models;

public record Party
{
    [Required]
    public required string HeadquartersAddress { get; init; }

    [Required]
    public required Guid PartyId { get; init; }
}
