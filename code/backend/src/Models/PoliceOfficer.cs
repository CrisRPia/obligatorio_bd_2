using System.ComponentModel.DataAnnotations;

namespace backend.src.Models;

public record PoliceOfficer : FullCitizen
{
    [Required]
    public required PoliceStation PoliceStation { get; init; }
}

public record PoliceStation
{
    [Required]
    public required Building Building { get; init; }
}
