using backend.src.Attributes;
using backend.src.Models;
using backend.src.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.src.Controllers;

[ApiController]
[Route("citizen/")]
public class CitizenController : Controller
{
    [HttpPost]
    [SafeAuthorize(roles: [Role.Voter])]
    [Route("{citizenId}/vote/{circuitId}/")]
    public async Task<DefaultOk> Vote(
        [UruguayanId] int citizenId,
        string circuitId,
        IncomingVotes votes
    )
    {
        throw new NotImplementedException();
    }
}
