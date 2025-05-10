using System.ComponentModel.DataAnnotations;
using backend.src.Attributes;
using backend.src.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.src.Controllers;

[ApiController]
[Route("citizen/")]
public class CitizenController : Controller
{
    [HttpPost]
    [Route("{citizenId}/vote/{circuitId}/")]
    public async Task<DefaultOk> Vote(
        [UruguayanIdAttribute] int citizenId,
        string circuitId,
        IncomingVotes votes
    )
    {
        throw new NotImplementedException();
    }
}
