using backend.src.Models;
using backend.src.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.src.Controllers;

[ApiController]
[Route("table/")]
public class TableController(
    ICitizenCacheService CitizenCache,
    IJwtService jwtService,
    IHttpContextAccessor httpContextAccessor
) : Controller
{
    [HttpPost]
    [SafeAuthorize(roles: [Role.BoardPresident])]
    [Route("{citizenId}/authorize/")]
    public async Task<BooleanReturn> Authorize(Ulid citizenId)
    {
        if (CitizenCache.GetCitizenCircuit(citizenId) is not null)
        {
            return BooleanReturn.True;
        }

        if (jwtService.GetData(httpContextAccessor)?.CircuitId is not Ulid circuitId)
        {
            return BooleanReturn.False;
        }

        CitizenCache.EnableCitizen(citizenId, circuitId);

        return BooleanReturn.True;
    }
}
