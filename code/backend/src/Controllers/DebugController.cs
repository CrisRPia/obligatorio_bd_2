using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using backend.src.Attributes;
using backend.src.Models;
using backend.src.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.src.Controllers;

[ApiController]
[Debug]
[Route("debug/")]
public class DebugController(IConfiguration Configuration) : Controller
{
    [HttpPost]
    [Route("fake")]
    public async Task<StateSnapshot> Fake(FakeInput input)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("fake/citizen")]
    public FullCitizen Fake() => FakeService.Instance.FakeCitizen();

    [HttpGet]
    [Route("fake/citizens")]
    public IEnumerable<FullCitizen> Fake(
        [FromQuery] int startUid,
        int endUid
    ) =>
        FakeService.Instance.FakeCitizens(
            Enumerable.Range(startUid, endUid - startUid)
        );

    [HttpGet]
    [Route("JWT")]
    [Authorize]
    public IEnumerable<string> GetTokenInfo()
    {
        var user = HttpContext.User;

        return user
            .Claims.Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value);
    }

    [HttpPost]
    [Route("JWT")]
    public AuthResponse<IEnumerable<Role>> FakeLogin(IEnumerable<Role> roles)
    {
        var jwt = new JwtService(Configuration);
        return new AuthResponse<IEnumerable<Role>>()
        {
            JwtToken = jwt.GenerateJwtToken("fake", "fake", roles),
            Content = roles,
        };
    }
}

public record FakeInput
{
    [Required]
    public required int CitizensToCreate { get; set; }

    [Required]
    public required int ElectionsToSimulate { get; set; }

    [Required]
    public required int CircuitsPerDepartment { get; set; }
    public IEnumerable<FullCitizen> PredeterminedCitizens { get; set; } = [];
}

public record StateSnapshot
{
    [Required]
    public required IEnumerable<Election> Elections { get; set; }
}
