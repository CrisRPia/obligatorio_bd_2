using backend.src.Attributes;
using backend.src.Models;
using backend.src.Queries;
using backend.src.Services;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace backend.src.Controllers;

[ApiController]
[Debug]
[Route("debug/")]
public class DebugController(IFakeService fake, ICitizenService citizen, IJwtService jwtService)
    : Controller
{
    [HttpPost]
    [Route("init")]
    public async Task<object?> Init()
    {
        var citizensToCreate = await Task.WhenAll(
            Enumerable.Range(0, 100).Select(async _ => await CreateCitizen())
        );

        var candidates = new Queue<CitizenCreationResult>(
            new Faker().Random.Shuffle(citizensToCreate)
        );

        var parties = new int[] { 2, 2, 2, 2, 2 }.Select(candidateCount => new Party()
        {
            HeadquartersAddress = new Faker().Address.StreetAddress(),
            PartyId = Ulid.NewUlid(),
            Citizens = Enumerable
                .Range(0, candidateCount)
                .Select(_ => candidates.Dequeue().FakeCitizen)
                .ToArray(),
        });

        foreach (var party in parties)
        {
            foreach (var (index, member) in party.Citizens.Index())
            {
                await DB.Queries.InsertPartyMember(
                    new()
                    {
                        AdmissionDate = new Faker()
                            .Date.RecentDateOnly(days: 1000)
                            .ToDateTime(TimeOnly.MinValue),
                        ExitDate = null,
                        Role = index switch
                        {
                            0 => Queries.Codegen.PartyHasCitizenRole.President,
                            1 => Queries.Codegen.PartyHasCitizenRole.VicePresident,
                            _ => throw new InvalidOperationException(),
                        },
                        PartyId = party.PartyId.ToByteArray(),
                    }
                );
            }

            // TODO : Insert ballots.
            // await DB.Queries.InsertBallot
        }

        return new { CreatedCitizens = citizensToCreate };
    }

    [HttpGet]
    [Route("fake/citizen")]
    public FullCitizen FakeCitizen() => fake.FakeCitizen();

    [HttpGet]
    [Route("fake/citizens")]
    public IEnumerable<FullCitizen> Fake([FromQuery] int startUid, [FromQuery] int endUid) =>
        fake.FakeCitizens(Enumerable.Range(startUid, endUid - startUid));

    [HttpPost]
    [Route("JWT")]
    public AuthResponse<ValueTuple> SetTokenInfo(IReadOnlyList<Role> roles)
    {
        var data = new EmbeddedJwtData()
        {
            CircuitId = new() { EstablishmentId = Ulid.NewUlid(), CircuitNumber = 1 },
            Roles = roles,
            TokenId = Ulid.NewUlid(),
            UserId = Ulid.NewUlid(),
            Username = "Fake username.",
        };

        var token = jwtService.GenerateJwtToken(data);
        return new AuthResponse<ValueTuple>()
        {
            JwtToken = token,
            CitizenId = data.UserId,
            User = ValueTuple.Create(),
            Roles = data.Roles,
        };
    }

    [HttpPost]
    [Route("Playground")]
    public async Task<object> Playground()
    {
        var output =
            await DB.Queries.SelectCitizen(
                new() { CitizenId = Ulid.Parse("01JWPVY084Q5JPB69N1MSG1V9N").ToByteArray() }
            ) ?? throw new Exception();

        Console.WriteLine(output);

        return new FullCitizen
        {
            CitizenId = new Ulid(output.CitizenId),
            BirthDate = DateOnly.FromDateTime(output.Birth),
            CredencialCivica = output.CredencialCivica,
            Name = output.Name,
            Surname = output.Surname,
            UruguayanId = output.UruguayanId,
        };
    }

    // Do this to prevent the long-ass encryption time.
    private string? PasswordCache { get; set; }

    [HttpPost]
    [Route("create_citizen")]
    public async Task<CitizenCreationResult> CreateCitizen()
    {
        var fakeCitizen = fake.FakeCitizen();
        PasswordCache ??= citizen.HashPassword("pato1234");

        var (id, commands) = citizen.InsertCitizen(fakeCitizen, PasswordCache);
        using var connection = DB.NewOpenConnection();
        var batch = connection.CreateBatch();

        foreach (var command in commands)
        {
            batch.BatchCommands.Add(command);
        }

        await batch.ExecuteNonQueryAsync();

        return new() { FakeCitizen = fakeCitizen, CitizenId = id };
    }
}

public record CitizenCreationResult()
{
    public required FullCitizen FakeCitizen { get; init; }
    public required Ulid CitizenId { get; init; }
}
