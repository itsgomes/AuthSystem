using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthSystem.Application.Abstractions.Security;
using AuthSystem.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthSystem.Infrastructure.Security;

public sealed class JwtAccessTokenGenerator(IOptions<JwtSettings> jwtOptions) : IAccessTokenGenerator
{
  public string Generate(User user, IReadOnlyCollection<string> permissions)
  {
    var settings = jwtOptions.Value;

    var claims = new List<Claim>
    {
      new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
      new(JwtRegisteredClaimNames.Email, user.Email),
      new(JwtRegisteredClaimNames.Name, user.Name),
      new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    claims.AddRange(
      permissions.Select(permission =>
        new Claim(JwtClaimNames.Permission, permission)));

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
      issuer: settings.Issuer,
      audience: settings.Audience,
      claims: claims,
      expires: DateTime.UtcNow.AddMinutes(settings.ExpirationInMinutes),
      signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}