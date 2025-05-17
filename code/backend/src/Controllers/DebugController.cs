using System.ComponentModel.DataAnnotations;
using backend.src.Attributes;
using backend.src.Models;
using backend.src.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.src.Controllers;

[ApiController]
[Debug]
[Route("debug/")]
public class DebugController(IConfiguration configuration) : Controller
{
    private IConfiguration Configuration => configuration;

    [HttpPost]
    [Route("Fake")]
    public async Task<StateSnapshot> Fake(FakeInput input)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("JWT")]
    public async Task<IEnumerable<Role>> GetTokenInfo()
    {
        // TODO
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("JWT")]
    public async Task<AuthResponse<IEnumerable<Role>>> FakeLogin(
        IEnumerable<Role> roles
    )
    {
        var jwt = new JwtService(Configuration);
        return new AuthResponse<IEnumerable<Role>>()
        {
            JwtToken = jwt.GenerateJwtToken("fake", "fake", roles),
            Content = roles,
        };
    }
}

public struct Unit { }

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
