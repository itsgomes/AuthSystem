using System.Net;
using System.Net.Http.Headers;
using AuthSystem.Api.IntegrationTests.Infrastructure;
using AuthSystem.Application.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AuthSystem.Api.IntegrationTests.Authorization;

public sealed class ProfileAuthorization(AuthSystemApiFactory factory) : IClassFixture<AuthSystemApiFactory>
{
  [Fact]
  public async Task Me_WithoutAccessToken_Returns401()
  {
    using var client = CreateClient();

    var response = await client.GetAsync("/api/profile/me");

    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
  }

  [Fact]
  public async Task Me_WithoutRequiredPermission_Returns403()
  {
    using var client = CreateClient();
    
    AddBearerToken(client, TestJwtGenerator.Generate());

    var response = await client.GetAsync("/api/profile/me");

    Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
  }

  [Fact]
  public async Task Me_WithRequiredPermission_Returns200()
  {
    using var client = CreateClient();
    
    var token = TestJwtGenerator.Generate(Permissions.ProfileRead);

    AddBearerToken(client, token);

    var response = await client.GetAsync("/api/profile/me");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
  }

  [Fact]
  public async Task Me_WithExpiredAccessToken_Returns401()
  {
    using var client = CreateClient();

    var token = TestJwtGenerator.Generate(
      permission: Permissions.ProfileRead,
      expiresAt: DateTime.UtcNow.AddMinutes(-1));

    AddBearerToken(client, token);

    var response = await client.GetAsync("/api/profile/me");

    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
  }

  [Fact]
  public async Task Me_WithTokenSignedByUnknownKey_Returns401()
  {
    using var client = CreateClient();

    var token = TestJwtGenerator.Generate(
      permission: Permissions.ProfileRead,
      secretKey: "unknown-test-secret-key-with-more-than-32-characters");

    AddBearerToken(client, token);

    var response = await client.GetAsync("/api/profile/me");

    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
  }

  private HttpClient CreateClient()
  {
    return factory.CreateClient(
      new WebApplicationFactoryClientOptions
      {
        BaseAddress = new Uri("https://localhost")
      });
  }

  private static void AddBearerToken(HttpClient client, string accessToken)
  {
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
  }
}