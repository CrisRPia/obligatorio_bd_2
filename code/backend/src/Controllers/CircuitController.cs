using backend.src.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.src.Controllers;

[ApiController]
[Route("departments/{departmentId}/circuit/{circuitId}")]
public class CircuitController : Controller
{
    [HttpGet]
    [Route("results/{electionId}")]
    public async Task<ElectionResult> Auth(Guid departmentId, int circuitId, Guid electionId)
    {
        throw new NotImplementedException();
    }

    [HttpPut]
    [Authorize("president")]
    [Route("authorize_vote/{voteId}")]
    public async Task<DefaultOk> Authorize(Guid departmentId, int circuitId, Guid electionId, Guid voteId) {
        throw new NotImplementedException();
    }
}
