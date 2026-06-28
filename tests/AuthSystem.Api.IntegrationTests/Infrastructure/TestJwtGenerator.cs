using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AuthSystem.Api.IntegrationTests.Infrastructure;

internal static class TestJwtGenerator
{
  public static string Generate(string? permission = null, string? secretKey = null, DateTime? expiresAt = null)
  {
    var claims = new List<Claim>
    {
      new(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
      new(JwtRegisteredClaimNames.Email, "test@example.com"),
      new(JwtRegisteredClaimNames.Name, "Test User")
    };

    if (permission is not null)
    {
      claims.Add(new Claim("permission", permission));
    }

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? AuthSystemApiFactory.SecretKey));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(
      issuer: "AuthSystem.Tests",
      audience: "AuthSystem.Tests",
      claims,
      expires: expiresAt ?? DateTime.UtcNow.AddMinutes(5),
      signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}