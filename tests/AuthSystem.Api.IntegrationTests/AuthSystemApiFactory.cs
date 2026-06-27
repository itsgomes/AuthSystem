using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace AuthSystem.Api.IntegrationTests;

public sealed class AuthSystemApiFactory : WebApplicationFactory<Program>
{
  public const string SecretKey = "integration-tests-secret-key-with-at-least-32-characters";

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseEnvironment("Testing");

    builder.ConfigureAppConfiguration((_, configuration) =>
    {
      configuration.AddInMemoryCollection(
        new Dictionary<string, string?>
        {
          ["Jwt:SecretKey"] = SecretKey,
          ["Jwt:Issuer"] = "AuthSystem.Tests",
          ["Jwt:Audience"] = "AuthSystem.Tests",
          ["Jwt:ExpirationInMinutes"] = "15"
        });
    });
  }
}