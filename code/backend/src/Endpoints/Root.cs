namespace backend.src.Endpoints;

public class Test
{
    public required string Test2 { get; init; }
    public required int Id { get; init; }
}

public static class Root
{
    public static RouteGroupBuilder MapPing(RouteGroupBuilder group)
    {
        group.MapGet("{id}", (int id) => id);
        group.MapPost("{id}", (int id, Test asd) =>
        {
            return asd;
        });

        return group;
    }
}
