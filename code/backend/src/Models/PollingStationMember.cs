using System.ComponentModel.DataAnnotations;

namespace backend.src.Models;

public record PollingStationMember : FullCitizen
{
    [Required]
    public required PollingStationMemberRole Role { get; init; }

    [Required]
    public required string Organization { get; init; }
}

public enum PollingStationMemberRole
{
    President,
    Secretary,
    Vocal,
}
