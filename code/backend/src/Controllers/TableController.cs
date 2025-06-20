using backend.src.Models;
using backend.src.Queries;
using backend.src.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.src.Controllers;

[ApiController]
[Route("table/")]
public class TableController(ICitizenCacheService CitizenCache, IJwtService jwtService) : Controller
{
    [HttpPost]
    [SafeAuthorize(roles: [Role.BoardPresident])]
    [Route("{citizenId}/authorize/")]
    public async Task<BooleanReturn> Authorize(Ulid citizenId, bool authorizeObserved)
    {
        if (CitizenCache.GetCitizenCircuit(citizenId) is not null)
        {
            return BooleanReturn.True;
        }

        if (jwtService.GetData(HttpContext)?.CircuitId is not CircuitId circuitId)
        {
            return BooleanReturn.False;
        }

        CitizenCache.EnableCitizen((citizenId, authorizeObserved), circuitId);

        return BooleanReturn.True;
    }

    [HttpPut]
    [SafeAuthorize(roles: [Role.BoardPresident])]
    [Route("close")]
    public async Task<BooleanReturn> CloseTable()
    {
        var data = jwtService.GetData(HttpContext);

        if (data is null || data.CircuitId is null)
        {
            return BooleanReturn.False;
        }

        var result = DB.Queries.UpdatePollingDistrict(
            new()
            {
                PollingDistrictNumber = data.CircuitId.CircuitNumber,
                EstablishmentId = data.CircuitId.EstablishmentId.ToByteArray(),
                IsOpen = false,
            }
        );
        return BooleanReturn.True;
    }
}
