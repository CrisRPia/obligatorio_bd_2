using System.ComponentModel.DataAnnotations;
using backend.src.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.src.Controllers;

[ApiController]
[Route("auth/")]
public class AuthController : Controller
{
    [HttpPost]
    [Route("polling_station/login")]
    public async Task<AuthResponse<PollingStationMember>> PollongStationLogin(
        BaseCitizen citizen
    )
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("polling_station/register")]
    public async Task<
        AuthResponse<PollingStationMember>
    > PollingStationRegister(PollingStationMember citizen)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("police_officer/register")]
    public async Task<AuthResponse<PoliceOfficer>> PoliceOfficerRegister(
        PollingStationMember citizen
    )
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("police_officer/login")]
    public async Task<AuthResponse<PoliceOfficer>> PoliceOfficerLogin(
        PollingStationMember citizen
    )
    {
        throw new NotImplementedException();
    }
}

public record AuthResponse<T>
{
    [Required]
    public required string JwtToken { get; set; }

    [Required]
    public required T Content { get; set; }
}
