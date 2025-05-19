using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using backend.src.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.src.Controllers;

[ApiController]
[Route("elections/")]
public class ElectionController : Controller
{
    [HttpGet]
    [Route("")]
    public async Task<ListModel<Election>> GetElections(
        [FromQuery] Filters filters
    )
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("{electionId}")]
    public async Task<Election> GetElection(string electionId)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("")]
    public async Task<DefaultOk> CreateElection(Election election)
    {
        throw new NotImplementedException();
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

    [Description("Inclusive.")]
    public DateTime? MaximumDateTime { get; init; }
    public string? DepartmentId { get; init; }
    public ElectionState? OnlyOpenOrClosed { get; init; }

    [Description("Do not specify to set to all.")]
    public IEnumerable<ElectionType> RestrictToTypes { get; init; } = [];
    public string? SearchTerm { get; init; }
    public bool? HasResults { get; init; }
    public string? AvailableForUser { get; init; }
}
