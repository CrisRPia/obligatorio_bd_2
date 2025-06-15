using System.ComponentModel.DataAnnotations;
using backend.src.Models;
using backend.src.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.src.Controllers;

[ApiController]
[Route("departments/{departmentId}/circuits/")]
public class CircuitController : Controller
{
    [HttpGet]
    [Route("{circuitId}/results/{electionId}")]
    public async Task<ElectionResult> GetResults(
        string departmentId,
        int circuitId,
        string electionId
    )
    {
        throw new NotImplementedException();
    }

    [HttpPut]
    [SafeAuthorize(roles: [Role.Admin, Role.BoardPresident])]
    [Route("{circuitId}/authorize_vote/{voteId}")]
    public async Task<BooleanReturn> AuthorizeVote(
        string departmentId,
        int circuitId,
        string electionId,
        string voteId
    )
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [EndpointDescription("Get all circuits within a deparment.")]
    public async Task<GetCircuitsReturn> GetAllCircuits(string departmentId)
    {
        throw new NotImplementedException();
    }
}

public record GetCircuitsReturn
{
    [Required]
    public required IEnumerable<Circuit> Circuits { get; init; }
}
