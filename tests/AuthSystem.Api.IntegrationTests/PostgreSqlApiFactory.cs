using AuthSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace AuthSystem.Api.IntegrationTests;

public sealed class PostgreSqlApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
  private readonly PostgreSqlContainer _postgres =
    new PostgreSqlBuilder("postgres:17-alpine")
      .WithDatabase("authsystem_tests")
      .WithUsername("postgres")
      .WithUsername("postgres")
      .Build();

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseEnvironment("Testing");

    builder.ConfigureAppConfiguration((_, configuration) =>
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

  async Task IAsyncLifetime.DisposeAsync()
  {
    await base.DisposeAsync();
    await _postgres.DisposeAsync();
  }
}