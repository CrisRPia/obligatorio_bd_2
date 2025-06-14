using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace backend.src.Models;

public partial record ListModel<T>
{
    [Required]
    public required IReadOnlyList<T> Items { get; init; }
}

public partial record ListModel<T> : IEnumerable<T>
{
    IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return Items.GetEnumerator();
    }
}
