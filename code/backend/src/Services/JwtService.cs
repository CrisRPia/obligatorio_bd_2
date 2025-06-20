using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.src.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;

namespace backend.src.Services;

public record EmbeddedJwtData
{
    public required string Username { get; init; }
    public required Ulid UserId { get; init; }
    public required Ulid? TokenId { get; init; }
    public CircuitId? CircuitId { get; init; }
    public required IEnumerable<Role> Roles { get; init; }
}

public interface IJwtService
{
    SecurityKey Key { get; }
    string Issuer { get; }
    string Audience { get; }
    EmbeddedJwtData? GetData(HttpContext httpContext);

    string GenerateJwtToken(EmbeddedJwtData data);
}

public class JwtService(IConfiguration configuration) : IJwtService
{
    private IConfiguration Configuration { get; init; } = configuration;
    private IConfigurationSection Section => Configuration.GetSection("Jwt");
    public SecurityKey Key => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Section["Key"]!));
    public string Issuer => Section["Issuer"]!;
    public string Audience => Section["Audience"]!;

    public string GenerateJwtToken(EmbeddedJwtData data)
    {
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Name, data.Username),
            new(JwtRegisteredClaimNames.Sub, data.UserId.ToString()),
            new(JwtRegisteredClaimNames.Jti, data.TokenId.ToString() ?? Ulid.NewUlid().ToString()),
        ];

        if (data.CircuitId is CircuitId content)
        {
            claims.Add(new(nameof(content.EstablishmentId), content.EstablishmentId.ToString()));
            claims.Add(new(nameof(content.CircuitNumber), content.CircuitNumber.ToString()));
        }

        foreach (var role in data.Roles)
            claims.Add(new Claim(ClaimTypes.Role, role.GetDisplayName()));

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(
            new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims, "jwt"),
                Issuer = Issuer,
                Audience = Audience,
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha512),
            }
        );

        return tokenHandler.WriteToken(securityToken);
    }

    public EmbeddedJwtData? GetData(HttpContext httpContextAccessor)
    {
        var user = httpContextAccessor.User;

        if (user.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var roles = user.FindAll(ClaimTypes.Role).Select(c => Enum.Parse<Role>(c.Value)).ToList();

        return new()
        {
            Roles = roles,
            Username =
                user.FindFirstValue(JwtRegisteredClaimNames.Name)
                ?? throw new InvalidOperationException(),
            UserId = Ulid.Parse(
                user.FindFirstValue(JwtRegisteredClaimNames.Sub)
                    ?? throw new InvalidOperationException()
            ),
            TokenId = Ulid.Parse(
                user.FindFirstValue(JwtRegisteredClaimNames.Jti)
                    ?? throw new InvalidOperationException()
            ),
            // I bet you can't write uglier code than this -- CR.
            CircuitId =
                user.FindFirstValue(nameof(EmbeddedJwtData.CircuitId.EstablishmentId))
                    is string establishmentId
                && user.FindFirstValue(nameof(EmbeddedJwtData.CircuitId.CircuitNumber))
                    is string circuitNumber
                    ? new()
                    {
                        EstablishmentId = Ulid.Parse(establishmentId),
                        CircuitNumber = int.Parse(circuitNumber),
                    }
                    : null,
        };
    }
}
