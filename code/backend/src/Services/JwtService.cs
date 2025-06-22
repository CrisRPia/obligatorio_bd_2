using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.src.Models;
using Microsoft.IdentityModel.Tokens;

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
    private static class CustomClaimTypes
    {
        public const string EstablishmentId = "establishment_id";
        public const string CircuitNumber = "circuit_number";
    }

    private IConfiguration Configuration { get; init; } = configuration;
    private IConfigurationSection Section => Configuration.GetSection("Jwt");
    public SecurityKey Key => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Section["Key"]!));
    public string Issuer => Section["Issuer"]!;
    public string Audience => Section["Audience"]!;

    public string GenerateJwtToken(EmbeddedJwtData data)
    {
        List<Claim> claims =
        [
            new(ClaimTypes.Name, data.Username),
            new(ClaimTypes.NameIdentifier, data.UserId.ToString()),
            new(JwtRegisteredClaimNames.Jti, (data.TokenId ?? Ulid.NewUlid()).ToString()),
        ];

        if (data.CircuitId is CircuitId content)
        {
            claims.Add(new(CustomClaimTypes.EstablishmentId, content.EstablishmentId.ToString()));
            claims.Add(new(CustomClaimTypes.CircuitNumber, content.CircuitNumber.ToString()));
        }

        foreach (var role in data.Roles)
            claims.Add(new Claim(ClaimTypes.Role, role.ToString()));

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

    private static CircuitId? CreateCircuitIdFromClaims(ClaimsPrincipal principal)
    {
        var establishmentIdStr = principal.FindFirstValue(CustomClaimTypes.EstablishmentId);
        var circuitNumberStr = principal.FindFirstValue(CustomClaimTypes.CircuitNumber);

        if (establishmentIdStr != null && circuitNumberStr != null)
        {
            return new CircuitId
            {
                EstablishmentId = Ulid.Parse(establishmentIdStr),
                CircuitNumber = int.Parse(circuitNumberStr),
            };
        }
        return null;
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
            Username = user.Identity.Name
                ?? throw new InvalidOperationException("No name found"),
            UserId = Ulid.Parse(
                    user.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new InvalidOperationException("No userId found")
            ),
            TokenId = Ulid.Parse(
                user.FindFirstValue(JwtRegisteredClaimNames.Jti)
                    ?? throw new InvalidOperationException("No tokenId found")
            ),
            CircuitId = CreateCircuitIdFromClaims(user),
        };
    }
}
