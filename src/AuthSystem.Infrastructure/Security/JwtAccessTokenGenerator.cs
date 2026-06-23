using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthSystem.Application.Abstractions.Security;
using AuthSystem.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthSystem.Infrastructure.Security;

public sealed class JwtAccessTokenGenerator(
  IOptions<JwtSettings> jwtOptions) : IAccessTokenGenerator
{
  public string Generate(User user)
  {
    var settings = jwtOptions.Value;

    var claims = new[]
    {
      new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
      new Claim(JwtRegisteredClaimNames.Email, user.Email),
      new Claim(JwtRegisteredClaimNames.Name, user.Name),
      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

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