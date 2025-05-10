using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace backend.src.Models;

public record Circuit
{
    [Required]
    public required int CircuitNumber { get; init; }

    [Required]
    public required Building Building { get; init; }
}

public record Building
{
    [Required]
    public required Guid BuildingId { get; init; }

    [Required]
    public required Zone Zone { get; init; }
}

public record Zone
{
    [Required]
    public required Guid ZoneId { get; init; }

    [Description("City, town or other.")]
    [Required]
    public required Locality Locality { get; init; }
}

public record Locality
{
    [Required]
    public required Guid LocalityId { get; init; }

    [Required]
    public required Department Department { get; init; }
}
