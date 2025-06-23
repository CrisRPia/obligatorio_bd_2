using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using backend.src.Models;
using backend.src.Queries;
using backend.src.Queries.Codegen;
using backend.src.Services;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Mvc;

namespace backend.src.Controllers;

[ApiController]
[Route("auth/")]
public class AuthController(IJwtService jwt) : Controller
{
    [HttpGet]
    [Route("JWT")]
    public EmbeddedJwtData? GetTokenInfo()
    {
        return jwt.GetData(HttpContext);
    }

    [Route("")]
    [HttpPost]
    public async Task<AuthResponse<FullCitizen>> Login(LoginCredentials citizen)
    {
        var select = await DB.Queries.LoginCitizen(
            new QueriesSql.LoginCitizenArgs
            {
                CredencialCivica = citizen.CredencialCivica,
                UruguayanId = citizen.UruguayanId,
            }
        );

        if (select is null || !Argon2.Verify(select.PasswordHash, citizen.Password))
            throw new NotImplementedException();

        var rolesMap = new Dictionary<Role, bool>
        {
            [Role.Admin] = true, // TODO
            [Role.Voter] = select.CitizenId is not null, // TODO
            [Role.Police] = select.PoliceOfficerId is not null,
            [Role.BoardPresident] = select.PollingStationPresidentId is not null,
            [Role.BoardSecretary] = select.PollingStationSecretaryId is not null,
            [Role.BoardVocal] = select.PollingStationVocalId is not null,
        };

        var roles = rolesMap.Where(kv => kv.Value).Select(kv => kv.Key).ToImmutableList();

        var token = jwt.GenerateJwtToken(
            new()
            {
                Username = $"{select.Name} {select.Surname}",
                UserId = new Ulid(select.CitizenId),
                Roles = roles,
                TokenId = null,
                CircuitId = select.PollingDistrictNumber is int pollingDistrictNumber
                    ? new()
                    {
                        EstablishmentId = new Ulid(select.EstablishmentId),
                        CircuitNumber = pollingDistrictNumber,
                    }
                    : null,
            }
        );

        return new AuthResponse<FullCitizen>
        {
            JwtToken = token,
            Roles = roles,
            CitizenId = new Ulid(select.CitizenId),
            User = new FullCitizen
            {
                CitizenId = new Ulid(select.CitizenId),
                BirthDate = DateOnly.FromDateTime(select.Birth),
                CredencialCivica = select.CredencialCivica,
                Name = select.Name,
                Surname = select.Surname,
                UruguayanId = select.UruguayanId,
            },
        };
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
    public required Ulid CitizenId { get; set; }

    [Required]
    public required T User { get; set; }
}
