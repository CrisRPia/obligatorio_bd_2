using System.ComponentModel.DataAnnotations;
using backend.src.Models;
using backend.src.Validators;
using Microsoft.AspNetCore.Mvc;

namespace backend.src.Controllers;

[ApiController]
[Route("citizen/")]
public class CitizenController : Controller
{
    [HttpPost]
    [Route("{citizenId}/vote/{circuitId}/")]
    public async Task<DefaultOk> Vote(
        [UruguayanIdValidator] int citizenId,
        string circuitId,
        IncomingVotes votes
    )
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("auth/polling_station/")]
    public async Task<PollingStationMemberAuthResponse> Auth(BaseCitizen citizen)
    {
        throw new NotImplementedException();
    }
}

public record PollingStationMemberAuthResponse {
    [Required] public required string JwtToken { get; set; }
    [Required] public required PollingStationMember Content { get; set; }
}
