using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace AuthSystem.Api.Extensions;

public static class AuthenticationExtensions
{
  public static IServiceCollection AddJwtAuthentication(
    this IServiceCollection services,
    IConfiguration configuration)
  {
    var jwtSettings = configuration.GetSection("Jwt");

    services
      .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(options =>
      {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = jwtSettings["Issuer"],
          ValidAudience = jwtSettings["Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
        };
      });

    return services;
  }
}