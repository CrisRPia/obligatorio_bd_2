using System.ComponentModel.DataAnnotations;

namespace backend.src.Models;

public record ListModel<T>
{
    [Required]
    public required IEnumerable<T> Items { get; init; }
}
