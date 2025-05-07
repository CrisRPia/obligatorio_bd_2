using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace backend.src.Controllers;

[ApiController]
[Route("[controller]/{ControllerId:guid}")]
public class ControllerController : ControllerBase
{
    [HttpPost]
    public async Task<Ok<string>> Create([FromBody] TestBody body, Guid ControllerId)
    {
        return TypedResults.Ok(body.Something + ControllerId.ToString());
    }

    [HttpGet]
    public IActionResult Get(Guid ControllerId)
    {
        return Ok(new TestBody(
            Something: ControllerId.ToString()
        ));
    }
}

public record TestBody(string Something) { }
