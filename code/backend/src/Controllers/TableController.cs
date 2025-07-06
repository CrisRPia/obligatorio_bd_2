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
        if (jwtService.GetData(HttpContext)?.CircuitId is not CircuitId circuitId)
        {
            return BooleanReturn.False;
        }

        var citizenAssignedCircuit = CitizenCache.GetCitizenCircuit(citizenId);

        if (citizenAssignedCircuit is not null) {
            return new() { Success = false, Message = "Citizen already assigned to a circuit." };
        }

        CitizenCache.EnableCitizen((citizenId, authorizeObserved), circuitId);

        return new() { Success = CitizenCache.GetCitizenCircuit(citizenId) is not null };
    }

    [HttpPut]
    [SafeAuthorize(roles: [Role.BoardPresident])]
    [Route("close")]
    public async Task<BooleanReturn> CloseTable() => await SetTo(false);

    [HttpPut]
    [SafeAuthorize(roles: [Role.BoardPresident])]
    [Route("open")]
    public async Task<BooleanReturn> OpenTable() => await SetTo(true);

    private async Task<BooleanReturn> SetTo(bool value)
    {
        var data = jwtService.GetData(HttpContext);

        if (data is null || data.CircuitId is null)
        {
            return BooleanReturn.False;
        }

        await DB.Queries.UpdatePollingDistrict(
            new()
            {
                PollingDistrictNumber = data.CircuitId.CircuitNumber,
                EstablishmentId = data.CircuitId.EstablishmentId.ToByteArray(),
                IsOpen = value,
            }
        );

        return BooleanReturn.True;
    }
}
