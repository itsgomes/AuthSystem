using AuthSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using RefreshTokenEntity = AuthSystem.Domain.Entities.RefreshToken;

namespace AuthSystem.Api.IntegrationTests.Infrastructure;

public sealed class PostgreSqlApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
  private readonly PostgreSqlContainer _postgres =
    new PostgreSqlBuilder("postgres:17-alpine")
      .WithDatabase("authsystem_tests")
      .WithUsername("postgres")
      .WithPassword("postgres")
      .Build();

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseEnvironment("Testing");

    builder.ConfigureAppConfiguration(
      (_, configuration) =>
      {
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
          ["ConnectionStrings:DefaultConnection"] = _postgres.GetConnectionString(),
          ["Jwt:SecretKey"] = AuthSystemApiFactory.SecretKey,
          ["Jwt:Issuer"] = "AuthSystem.Tests",
          ["Jwt:Audience"] = "AuthSystem.Tests",
          ["Jwt:ExpirationInMinutes"] = "15"
        });
      });
  }

  public async Task InitializeAsync()
  {
    await _postgres.StartAsync();

    await using var scope = Services.CreateAsyncScope();

    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await dbContext.Database.MigrateAsync();
  }

  public async Task DeactivateUserAsync(string email)
  {
    await using var scope = Services.CreateAsyncScope();

    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var user = await dbContext.Users.SingleAsync(user => user.Email == email);

    user.Deactivate();

    await dbContext.SaveChangesAsync();
  }

  public async Task<RefreshTokenEntity?> GetRefreshTokenAsync(string token)
  {
    await using var scope = Services.CreateAsyncScope();

    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    return await dbContext.RefreshTokens
      .AsNoTracking()
      .SingleOrDefaultAsync(refreshToken => refreshToken.Token == token);
  }

  public async Task<IReadOnlyList<RefreshTokenEntity>> GetRefreshTokensByUserIdAsync(Guid userId)
  {
    await using var scope = Services.CreateAsyncScope();

    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    return await dbContext.RefreshTokens
      .AsNoTracking()
      .Where(refreshToken => refreshToken.UserId == userId)
      .OrderBy(refreshToken => refreshToken.CreatedAt)
      .ToListAsync();
  }

  async Task IAsyncLifetime.DisposeAsync()
  {
    await DisposeAsync();
    await _postgres.DisposeAsync();
  }
}