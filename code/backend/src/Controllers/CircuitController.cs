using System.ComponentModel.DataAnnotations;
using backend.src.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.src.Controllers;

[ApiController]
[Route("departments/{departmentId}/circuits/")]
public class CircuitController : Controller
{
    [HttpGet]
    [Route("{circuitId}/results/{electionId}")]
    public async Task<ElectionResult> GetResults(Guid departmentId, int circuitId, Guid electionId)
    {
        throw new NotImplementedException();
    }

    [HttpPut]
    [Authorize("president")]
    [Route("{circuitId}/authorize_vote/{voteId}")]
    public async Task<DefaultOk> AuthorizeVote(Guid departmentId, int circuitId, Guid electionId, Guid voteId) {
        throw new NotImplementedException();
    }

    [HttpGet]
    [EndpointDescription("Get all circuits within a deparment.")]
    public async Task<GetCircuitsReturn> GetAllCircuits(Guid departmentId) {
        throw new NotImplementedException();
    }
}

public record GetCircuitsReturn {
    [Required] public required IEnumerable<Circuit> Circuits { get; init; }
}
