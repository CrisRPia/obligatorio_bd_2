using System.ComponentModel.DataAnnotations;
using backend.src.Attributes;
using backend.src.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.src.Controllers;

[ApiController]
[Route("debug/")]
public class DebugController : Controller
{
    [HttpPost]
    [Debug]
    [Route("Fake")]
    public async Task<StateSnapshot> Vote(FakeInput input)
    {
        throw new NotImplementedException();
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
