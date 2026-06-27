using AuthSystem.Application.Abstractions.Security;
using AuthSystem.Application.Authorization;

namespace AuthSystem.Api.Extensions;

public static class AuthorizationExtensions
{
  public static IServiceCollection AddPolicyAuthorization(
    this IServiceCollection services)
  {
    services.AddAuthorizationBuilder()
      .AddPolicy(Policies.ProfileRead, policy =>
      {
        policy.RequireClaim(
          JwtClaimNames.Permission,
          Permissions.ProfileRead);
      });

    return services;
  }
}