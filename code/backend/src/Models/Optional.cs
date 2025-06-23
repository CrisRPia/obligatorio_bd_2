namespace backend.src.Models;

public record Option<T>
{
    public T? Value { get; init; }

    public static Option<T> None => new() { };

    public static Option<T> Some(T value) => new() { Value = value };
}
