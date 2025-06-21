using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace backend.src.Models;

public record CircuitId {
    public required int CircuitNumber { get; init; }
    public required Ulid EstablishmentId { get; init; }
}

public record Circuit
{
    [Required]
    public required CircuitId CircuitId { get; init; }

    [Required]
    public required Building Building { get; init; }
}

public record Building
{
    [Required]
    public required Ulid BuildingId { get; init; }

    [Required]
    public required string Name { get; init; }

    [Required]
    public required string Address { get; init; }

    [Required]
    public required Zone Zone { get; init; }
}

public record Zone
{
    [Required]
    public required Ulid ZoneId { get; init; }

    [Description("City, town or other.")]
    [Required]
    public required Locality Locality { get; init; }
}

public enum LocalityType
{
    City,
    Town,
    Other,
}

public record Locality
{
    [Required]
    public required Ulid LocalityId { get; init; }

    [Required]
    public required Department Department { get; init; }

    [Required]
    public required LocalityType Type { get; init; }
}
