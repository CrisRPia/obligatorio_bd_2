using System.ComponentModel.DataAnnotations;

namespace backend.src.Models;

public record Department
{
    [Required]
    public required string Name { get; init; }

    [Required]
    public required Ulid DepartmentId { get; init; }
}
