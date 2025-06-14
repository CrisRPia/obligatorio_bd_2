using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;

namespace backend.src.Services;

public interface IJwtService
{
    SecurityKey Key { get; }
    string Issuer { get; }
    string Audience { get; }

    string GenerateJwtToken(string username, string userId, IEnumerable<Role> roles);
}

public class JwtService(IConfiguration configuration) : IJwtService
{
    private IConfiguration Configuration { get; init; } = configuration;
    private IConfigurationSection Section => Configuration.GetSection("Jwt");
    public SecurityKey Key => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Section["Key"]!));
    public string Issuer => Section["Issuer"]!;
    public string Audience => Section["Audience"]!;

    public string GenerateJwtToken(string username, string userId, IEnumerable<Role> roles)
    {
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Name, username),
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        ];

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role.GetDisplayName()));

        // 6. Create the JWT token
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
}
