using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;

namespace backend.src.Services;

public record EmbeddedJwtData
{
    public required string Username { get; init; }
    public required string UserId { get; init; }
    public required string? TokenId { get; init; }
    public Ulid? CircuitId { get; init; }
    public required IEnumerable<Role> Roles { get; init; }
}

public interface IJwtService
{
    SecurityKey Key { get; }
    string Issuer { get; }
    string Audience { get; }
    EmbeddedJwtData? GetData(IHttpContextAccessor httpContextAccessor);

    string GenerateJwtToken(EmbeddedJwtData data);
}

public class JwtService(IConfiguration configuration)
    : IJwtService
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
            new(JwtRegisteredClaimNames.Sub, data.UserId),
            new(JwtRegisteredClaimNames.Jti, data.TokenId ?? Guid.NewGuid().ToString()),
        ];

        if (data.CircuitId is Ulid content)
        {
            claims.Add(new(nameof(data.CircuitId), content.ToString()));
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
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha512),
            }
        );

        return tokenHandler.WriteToken(securityToken);
    }

    public EmbeddedJwtData? GetData(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var roles = user.FindAll(ClaimTypes.Role).Select(c => Enum.Parse<Role>(c.Value)).ToList();

        try
        {
            return new()
            {
                Roles = roles,
                Username =
                    user.FindFirstValue(JwtRegisteredClaimNames.Name)
                    ?? throw new InvalidOperationException(),
                UserId =
                    user.FindFirstValue(JwtRegisteredClaimNames.Sub)
                    ?? throw new InvalidOperationException(),
                TokenId =
                    user.FindFirstValue(JwtRegisteredClaimNames.Jti)
                    ?? throw new InvalidOperationException(),
                CircuitId = Ulid.Parse(
                    user.FindFirstValue(nameof(EmbeddedJwtData.CircuitId))
                        ?? throw new InvalidOperationException()
                ),
            };
        }
        catch (Exception ex)
        {
            throw new SecurityTokenInvalidIssuerException("Failed to parse claims from token.", ex);
        }
    }
}
