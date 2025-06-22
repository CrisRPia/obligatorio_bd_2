using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
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
    private static int ListNumber { get; set; } = 420;
    private static int CircuitNumber { get; set; } = 69;
    private static Faker Faker { get; } = new Faker();

    [HttpPost]
    [Route("fake/init")]
    public async Task<FakeInitResult> Init()
    {
        var deparments = await new DepartmentController().InitDepartments();
        var montevideo = deparments.Where(d => d.Name == "Montevideo").First();

        var citizensToCreate = (
            await Task.WhenAll(Enumerable.Range(0, 100).Select(async _ => await CreateCitizen()))
        )
            .Select(c => c.FakeCitizen)
            .ToList();

        var RandomCitizenPicker = new Queue<FullCitizen>(
            new Faker().Random.Shuffle(citizensToCreate)
        );

        var parties = new int[] { 2, 2, 2, 2, 2 }
            .Select(candidateCount => new Party()
            {
                HeadquartersAddress = new Faker().Address.StreetAddress(),
                PartyId = Ulid.NewUlid(),
                Name = Faker.Company.CompanyName(),
                Citizens = Enumerable
                    .Range(0, candidateCount)
                    .Select(_ => RandomCitizenPicker.Dequeue())
                    .ToArray(),
            })
            .ToList();

        var electionId = Ulid.NewUlid();

        var locality = new Locality()
        {
            LocalityId = Ulid.NewUlid(),
            Department = montevideo,
            Type = LocalityType.City,
        };


        // Insert establishment
        await DB.Queries.InsertLocality(
            new()
            {
                DepartmentId = locality.Department.DepartmentId.ToByteArray(),
                LocalityId = locality.LocalityId.ToByteArray(),
                Name = "Nombredelocalidad",
                Type = locality.Type switch
                {
                    LocalityType.City => Queries.Codegen.LocalityType.City,
                    LocalityType.Town => Queries.Codegen.LocalityType.Town,
                    LocalityType.Other => Queries.Codegen.LocalityType.Other,
                },
            }
        );

        await DB.Queries.InsertElection(
            new()
            {
                Date = DateTime.Today,
                Description = "Descripcion de eleccion",
                ElectionId = electionId.ToByteArray(),
            }
        );

        await DB.Queries.InsertMunicipalElection(new() {
                ElectionId = electionId.ToByteArray(),
                LocalityId = locality.LocalityId.ToByteArray(),
        });

        foreach (var party in parties)
        {
            // Insert party
            await DB.Queries.InsertParty(
                new()
                {
                    HedquartersAdress = party.HeadquartersAddress,
                    Name = party.Name,
                    PartyId = party.PartyId.ToByteArray(),
                }
            );

            // Insert partyMembers
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

            // Insert ballot
            var ballotId = Ulid.NewUlid();
            await DB.Queries.CreateBallot(new() { BallotId = ballotId.ToByteArray() });

            await DB.Queries.CreateListBallot(
                new() { ListBallotId = ballotId.ToByteArray(), ListNumber = ListNumber++ }
            );

            await DB.Queries.AddListBallotToDepartment(
                new()
                {
                    DeparmentId = montevideo.DepartmentId.ToByteArray(),
                    ListId = ballotId.ToByteArray(),
                }
            );

            await DB.Queries.AllowBallotInElection(
                new() { BallotId = ballotId.ToByteArray(), ElectionId = electionId.ToByteArray() }
            );
        }

        var establishment = new Building
        {
            Address = Faker.Address.FullAddress(),
            BuildingId = Ulid.NewUlid(),
            Name = "nombredeedificio",
            Zone = new() { ZoneId = Ulid.NewUlid(), Locality = locality },
        };

        await DB.Queries.InsertZone(
            new()
            {
                LocalityId = establishment.Zone.Locality.LocalityId.ToByteArray(),
                Name = "Nombredezona",
                PostalCode = Faker.Address.ZipCode(),
                ZoneId = establishment.Zone.ZoneId.ToByteArray(),
            }
        );

        await DB.Queries.InsertEstablishment(
            new()
            {
                Address = establishment.Address,
                EstablishmentId = establishment.BuildingId.ToByteArray(),
                Name = establishment.Name,
                ZoneId = establishment.Zone.ZoneId.ToByteArray(),
            }
        );

        // Insert circuits
        var circuits = new[] { 0, 0 }
            .Select(
                (_) =>
                    new Circuit
                    {
                        CircuitId = new()
                        {
                            CircuitNumber = ++CircuitNumber,
                            EstablishmentId = establishment.BuildingId,
                        },
                        Building = establishment,
                    }
            )
            .ToImmutableArray();

        var tables = circuits
            .Select(
                (_) =>
                    new Table
                    {
                        President = RandomCitizenPicker.Dequeue(),
                        Secretary = RandomCitizenPicker.Dequeue(),
                        Vocal = RandomCitizenPicker.Dequeue(),
                    }
            )
            .ToList();

        foreach (var (circuit, table) in circuits.Zip(tables))
        {
            // Fake circuit
            await DB.Queries.InsertCircuit(
                new()
                {
                    EstablishmentId = circuit.CircuitId.EstablishmentId.ToByteArray(),
                    PollingDistrictNumber = circuit.CircuitId.CircuitNumber,
                }
            );

            // Add members
            await DB.Queries.InsertBoardPresident(
                new()
                {
                    Org = Faker.Company.CompanyName(),
                    PollingStationPresidentId = table.President.CitizenId.ToByteArray(),
                }
            );
            await DB.Queries.InsertBoardSecretary(
                new()
                {
                    Org = Faker.Company.CompanyName(),
                    PollingStationSecretaryId = table.Secretary.CitizenId.ToByteArray(),
                }
            );
            await DB.Queries.InsertBoardVocal(
                new()
                {
                    Org = Faker.Company.CompanyName(),
                    PollingStationVocalId = table.Vocal.CitizenId.ToByteArray(),
                }
            );

            // Set members to circuit
            await DB.Queries.InsertBoardInCircuitElection(
                new()
                {
                    ElectionId = electionId.ToByteArray(),
                    EstablishmentId = circuit.CircuitId.EstablishmentId.ToByteArray(),
                    PollingDistrictNumber = circuit.CircuitId.CircuitNumber,
                    PollingStationPresidentId = table.President.CitizenId.ToByteArray(),
                    PollingStationSecretaryId = table.Secretary.CitizenId.ToByteArray(),
                    PollingStationVocalId = table.Vocal.CitizenId.ToByteArray(),
                }
            );
        }

        var activeCircuit = circuits[0];

        foreach (var citizen in citizensToCreate)
        {
            await DB.Queries.AssignCitizenIntoPollingDistrictElection(
                new()
                {
                    CitizenId = citizen.CitizenId.ToByteArray(),
                    ElectionId = electionId.ToByteArray(),
                    EstablishmentId = activeCircuit.CircuitId.EstablishmentId.ToByteArray(),
                    PollingDistrictNumber = activeCircuit.CircuitId.CircuitNumber,
                }
            );
        }

        return new()
        {
            ActiveCircuit = activeCircuit,
            Building = establishment,
            Circuits = circuits,
            CreatedCitizens = citizensToCreate,
            ElectionId = electionId,
            Parties = parties,
            Tables = tables,
        };
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

public record FakeInitResult
{
    // return new { citizensToCreate, electionId, parties, circuits, activeCircuit, tables, establishment };
    [Required]
    public required IReadOnlyList<FullCitizen> CreatedCitizens { get; init; }

    [Required]
    public required Ulid ElectionId { get; init; }

    [Required]
    public required IReadOnlyList<Party> Parties { get; init; }

    [Required]
    public required IReadOnlyList<Circuit> Circuits { get; init; }

    [Required]
    public required Circuit ActiveCircuit { get; init; }

    [Required]
    public required IReadOnlyList<Table> Tables { get; init; }

    [Required]
    public required Building Building { get; init; }
}

public record Table
{
    [Required]
    public required FullCitizen President { get; init; }

    [Required]
    public required FullCitizen Secretary { get; init; }

    [Required]
    public required FullCitizen Vocal { get; init; }
}

public record CitizenCreationResult
{
    public required FullCitizen FakeCitizen { get; init; }
    public required Ulid CitizenId { get; init; }
}
