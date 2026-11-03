using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Morimens.Infrastructure.Identity;

namespace Morimens.Api.Services;

public class JwtService
{
    private readonly IConfiguration _cfg;

    public JwtService(IConfiguration cfg) => _cfg = cfg;

    public (string token, int expiresIn) GenerateAccessToken(ApplicationUser user)
    {
        var secret  = _cfg["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret not configured.");
        var issuer  = _cfg["Jwt:Issuer"]  ?? "morimens-api";
        var audience = _cfg["Jwt:Audience"] ?? "morimens-clients";
        var expiresIn = int.TryParse(_cfg["Jwt:ExpiresInSeconds"], out var s) ? s : 900; // 15 min default

        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer:             issuer,
            audience:           audience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddSeconds(expiresIn),
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresIn);
    }
}
