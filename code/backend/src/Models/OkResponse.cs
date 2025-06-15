namespace backend.src.Models;

public record BooleanReturn()
{
    public required bool Success { get; init; }
    public static readonly BooleanReturn True = new() { Success = true };
    public static readonly BooleanReturn False = new() { Success = false };
}
