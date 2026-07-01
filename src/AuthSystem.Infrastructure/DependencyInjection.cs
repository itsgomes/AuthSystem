using AuthSystem.Application.Abstractions.Persistence;
using AuthSystem.Application.Abstractions.Security;
using AuthSystem.Infrastructure.Persistence;
using AuthSystem.Infrastructure.Repositories;
using AuthSystem.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthSystem.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddDbContext<AppDbContext>(options =>
    {
      options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
    });

    services
      .AddOptions<JwtSettings>()
      .Bind(configuration.GetSection("Jwt"))
      .Validate(settings => !string.IsNullOrWhiteSpace(settings.SecretKey), "JWT SecretKey is required.")
      .Validate(settings => settings.SecretKey.Length >= 32, "JWT SecretKey must have at least 32 characters.")
      .Validate(settings => settings.ExpirationInMinutes > 0, "JWT expiration must be greater than zero.")
      .ValidateOnStart();

    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IRoleRepository, RoleRepository>();
    services.AddScoped<IPermissionRepository, PermissionRepository>();
    services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    services.AddScoped<IPasswordHasher, PasswordHasher>();
    services.AddScoped<IAccessTokenGenerator, JwtAccessTokenGenerator>();
    services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
    services.AddScoped<IRefreshTokenHasher, RefreshTokenHasher>();

    return services;
  }
}