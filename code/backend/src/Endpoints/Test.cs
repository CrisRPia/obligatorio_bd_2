using Microsoft.AspNetCore.Mvc;

namespace backend.src.Endpoints;

public static class TestEndpoints {
    public static RouteGroupBuilder MapTestEndpoints(this RouteGroupBuilder group) {
        group.MapGet("/{id}", GetTest);
        group.MapPost("/", PostTest);
        return group;
    }

    public static IResult GetTest(int id) {
        return TypedResults.Ok("Hi");
    }

    public static IResult PostTest([FromBody] string str) {
        return TypedResults.NotFound("What the dog doin.");
    }
}
