namespace backend.src.Models;

public record DefaultOk()
{
    public required bool Success { get; init; }
    public static readonly DefaultOk Instance = new() { Success = true };
}
