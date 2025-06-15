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

        if (filters.SearchTerm is not null)
        {
            result = result.Where(e => e.Description.Contains(filters.SearchTerm));
        }

        if (filters.HasResults is not null)
        {
            result = result.Where(e => !e.IsOpen);
        }

        IEnumerable<Election> parsedResults = result.Select(e =>
        {
            return new Election()
            {
                Date = DateOnly.FromDateTime(e.Date),
                DepartmentId = e.DepartmentId is not null ? new Ulid(e.DepartmentId) : null,
                // TODO: Get allowed ballots.
                AllowedBallots = [],
                // TODO: Get result information
                Result = null,
                ElectionId = new Ulid(e.ElectionId),
                Type = e switch
                {
                    { BallotageId: byte[] } => ElectionType.Runoff,
                    { MunicipalId: byte[] } => ElectionType.MunicipalElection,
                    { PresidentialId: byte[] } => ElectionType.Presidential,
                    { PleibisciteId: byte[] } => ElectionType.Plebiscite,
                    { ReferndumId: byte[] } => ElectionType.Referendum,
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

        return new() { Items = parsedResults.ToList() };
    }
}

public enum ElectionState
{
    Open,
    Closed,
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
