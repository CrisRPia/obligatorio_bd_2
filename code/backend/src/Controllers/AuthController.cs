using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using backend.src.Models;
using backend.src.Queries;
using backend.src.Services;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Mvc;

namespace backend.src.Controllers;

[ApiController]
[Route("auth/")]
public class AuthController(IJwtService jwt, IAuthService authService) : Controller
{
    [HttpGet]
    [Route("JWT")]
    public EmbeddedJwtData? GetTokenInfo()
    {
        return jwt.GetData(HttpContext);
    }

    [Route("voter")]
    [HttpPost]
    public async Task<AuthResponse<BooleanReturn>> LoginVoter(LoginCredentials citizen) {
        var result = await authService.Login(citizen);

        var token = jwt.GenerateJwtToken(
            new()
            {
                Username = "Votante",
                UserId = result.User.CitizenId,
                Roles = [Role.Voter],
                TokenId = null,
                CircuitId = result.Circuit.CircuitId
            }
        );

        return new() {
            Circuit = result.Circuit,
            JwtToken = token,
            Roles = [Role.Voter],
            User = BooleanReturn.True,
        };
    }

    [Route("table")]
    [HttpPost]
    public async Task<AuthResponse<FullCitizen>> LoginTable(LoginCredentials citizen) {
        var result = await authService.Login(citizen);
        var token = jwt.GenerateJwtToken(
            new()
            {
                Username = $"{result.User.Name} {result.User.Surname}",
                UserId = result.User.CitizenId,
                Roles = result.Roles,
                TokenId = null,
                CircuitId = result.Circuit.CircuitId
            }
        );

        return result with { JwtToken = token };
    }
}

public record LoginCredentials : BaseCitizen
{
    [Required]
    public required string Password { get; set; }
}

public record AuthResponse<T>
{
    [Required]
    public required string JwtToken { get; set; }

    [Required]
    public required IEnumerable<Role> Roles { get; set; }

    [Required]
    public required T User { get; set; }

    [Required]
    public required Circuit Circuit { get; set; }
}
