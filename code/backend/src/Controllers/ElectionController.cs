using System.ComponentModel;
using backend.src.Models;
using backend.src.Queries;
using Microsoft.AspNetCore.Mvc;

namespace backend.src.Controllers;

[ApiController]
[Route("elections/")]
public class ElectionController : Controller
{
    [HttpGet]
    [Route("")]
    public async Task<ListModel<Election>> GetElections([FromQuery] Filters filters)
    {
        // Get elections
        IEnumerable<Queries.Codegen.QueriesSql.GetElectionsForCitizenRow> result =
            await DB.Queries.GetElectionsForCitizen(
                new()
                {
                    CitizenId = filters.AvailableForUser.ToByteArray(),
                    EndDate = filters.MaximumDateTime ?? DateTime.Today.Add(TimeSpan.FromDays(365)),
                    StartDate =
                        filters.MinimumDateTime ?? DateTime.Today.Subtract(TimeSpan.FromDays(365)),
                }
            );

        // Filter by optional filters.
        if (filters.SearchTerm is not null)
        {
            result = result.Where(e => e.Description.Contains(filters.SearchTerm));
        }

        if (filters.HasResults is not null)
        {
            result = result.Where(e => e.State == Queries.Codegen.ElectionState.Closed);
        }

        IEnumerable<Election> parsedResults = result.Select(e =>
        {
            return new Election()
            {
                Date = DateOnly.FromDateTime(e.Date),
                DepartmentId = e.DepartmentId is not null ? new Ulid(e.DepartmentId) : null,
                // Allowed ballots are calculated later on.
                AllowedBallots = [],
                State = e.State switch
                {
                    Queries.Codegen.ElectionState.Open => ElectionState.Open,
                    Queries.Codegen.ElectionState.Closed => ElectionState.Closed,
                    Queries.Codegen.ElectionState.NotStarted => ElectionState.NotStarted,
                    Queries.Codegen.ElectionState.Invalid => throw new InvalidOperationException(),
                },
                ElectionId = new Ulid(e.ElectionId),
                Type = e switch
                {
                    { BallotageId: byte[] } => ElectionType.Runoff,
                    { MunicipalId: byte[] } => ElectionType.MunicipalElection,
                    { PresidentialId: byte[] } => ElectionType.Presidential,
                    { PleibisciteId: byte[] } => ElectionType.Plebiscite,
                    { ReferendumId: byte[] } => ElectionType.Referendum,
                    _ => throw new InvalidOperationException(),
                },
            };
        });

        if (filters.RestrictToTypes.Any())
        {
            parsedResults = parsedResults.Where(e => filters.RestrictToTypes.Contains(e.Type));
        }

        if (filters.DepartmentId is not null)
        {
            parsedResults = parsedResults.Where(e => e.DepartmentId == filters.DepartmentId);
        }

        // After being done with all filtering, make a second query to fetch the elections.
        var rows = await DB.Queries.GetBallotsForElections(
            new() { Elections = parsedResults.Select(r => r.ElectionId.ToByteArray()).ToArray() }
        );

        parsedResults = parsedResults.Select(result =>
            result with
            {
                AllowedBallots = rows.Where(row =>
                        row.ElectionId.SequenceEqual(result.ElectionId.ToByteArray())
                    )
                    .Select(row => new Ballot()
                    {
                        ListNumber = row.ListNumber,
                        IsYes = row.Value,
                        ElectionId = result.ElectionId,
                        BallotId = new Ulid(row.BallotId),
                    })
                    .ToArray(),
            }
        );

        return new() { Items = parsedResults.ToList() };
    }

    [HttpPost]
    [Route("result")]
    public async Task<ListModel<ElectionResult?>> GetElectionResult(IReadOnlyList<Ulid> electionIds)
    {
        var result = await DB.Queries.GetMunicipalElectionResult(
            new() { Elections = electionIds.Select(e => e.ToByteArray()).ToArray() }
        );

        var elections = result.GroupBy(row => new Ulid(row.ElectionId));

        return new()
        {
            Items = elections
                .Select(group =>
                {
                    return new ElectionResult()
                    {
                        Type = ElectionType.MunicipalElection,
                        TotalVotes = (int)group.Select(row => row.AmountOfVotes).Sum(),
                        ListBasedResult = group
                            .Select(row => new VoteResult<Ballot>()
                            {
                                VoteCount = (int)row.AmountOfVotes,
                                Vote = new()
                                {
                                    ListNumber = row.ListNumber,
                                    BallotId = new Ulid(row.ListBallotId),
                                    ElectionId = new Ulid(row.ElectionId),
                                },
                            })
                            .ToList(),
                    };
                })
                .ToArray(),
        };
    }
}

public record Filters
{
    public DateTime? MinimumDateTime { get; init; }
    public DateTime? MaximumDateTime { get; init; }
    public Ulid? DepartmentId { get; init; }
    public ElectionState? OnlyOpenOrClosed { get; init; }

    [Description("Do not specify (or empty) to set to all.")]
    public IReadOnlyList<ElectionType> RestrictToTypes { get; init; } = [];
    public string? SearchTerm { get; init; }
    public bool? HasResults { get; init; }
    public Ulid AvailableForUser { get; init; }
}
