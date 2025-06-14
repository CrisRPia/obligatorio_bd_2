using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using backend.src.Attributes;
using backend.src.Models;
using backend.src.Queries;
using backend.src.Queries.Codegen;
using backend.src.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.src.Controllers;

[ApiController]
[Debug]
[Route("debug/")]
public class DebugController(IFakeService fake, ICitizenService citizen) : Controller
{
    [HttpPost]
    [Route("fake")]
    public async Task<StateSnapshot> Fake(FakeInput input)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("fake/citizen")]
    public FullCitizen Fake() => fake.FakeCitizen();

    [HttpGet]
    [Route("fake/citizens")]
    public IEnumerable<FullCitizen> Fake([FromQuery] int startUid, [FromQuery] int endUid) =>
        fake.FakeCitizens(Enumerable.Range(startUid, endUid - startUid));

    [HttpGet]
    [Route("JWT")]
    [Authorize]
    public IEnumerable<string> GetTokenInfo()
    {
        var user = HttpContext.User;

        return user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
    }

    [HttpPost]
    [Route("Playground")]
    public async Task<object> Playground()
    {
        var output =
            await DB.Queries.SelectCitizen(
                new QueriesSql.SelectCitizenArgs
                {
                    CitizenId = Ulid.Parse("01JWPVY084Q5JPB69N1MSG1V9N").ToByteArray(),
                }
            ) ?? throw new Exception();

        Console.WriteLine(output);

        return new FullCitizen
        {
            BirthDate = DateOnly.FromDateTime(output.Birth),
            CredencialCivica = output.CredencialCivica,
            Name = output.Name,
            Surname = output.Surname,
            UruguayanId = output.UruguayanId,
        };
    }

    [HttpPost]
    [Route("create_citizen")]
    public async Task<object> CreateCitizen()
    {
        var fakeCitizen = fake.FakeCitizen();
        var password = "pato1234";

        var (id, commands) = citizen.InsertCitizen(fakeCitizen, citizen.HashPassword(password));
        using var connection = DB.NewOpenConnection();
        var batch = connection.CreateBatch();

        foreach (var command in commands)
        {
            batch.BatchCommands.Add(command);
        }

        await batch.ExecuteNonQueryAsync();

        return new { fakeCitizen, id };
    }
}

public record FakeInput
{
    [Required]
    public required int CitizensToCreate { get; set; }

    [Required]
    public required int ElectionsToSimulate { get; set; }

    [Required]
    public required int CircuitsPerDepartment { get; set; }
    public IEnumerable<FullCitizen> PredeterminedCitizens { get; set; } = [];
}

public record StateSnapshot
{
    [Required]
    public required IEnumerable<Models.Election> Elections { get; set; }
}
