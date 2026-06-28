using System.Net;
using System.Net.Http.Json;
using AuthSystem.Api.IntegrationTests.Infrastructure;
using AuthSystem.Application.UseCases.Users.LoginUser;
using AuthSystem.Application.UseCases.Users.RefreshToken;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AuthSystem.Api.IntegrationTests.Authentication;

[Collection(PostgreSqlCollection.Name)]
public sealed class InactiveUserTests(PostgreSqlApiFactory factory)
{
  private const string Password = "Password@123";

  [Fact]
  public async Task Login_WithInactiveUser_Returns401()
  {
    using var client = CreateClient();
    var email = CreateUniqueEmail();

    await RegisterUserAsync(client, email);
    await factory.DeactivateUserAsync(email);

    var response = await client.PostAsJsonAsync(
      "/api/auth/login", 
      new LoginUserRequest(email, Password));

    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
  }

  [Fact]
  public async Task Refresh_WithInactiveUser_Returns401()
  {
    using var client = CreateClient();
    var email = CreateUniqueEmail();

    await RegisterUserAsync(client, email);

    var loginResponse = await client.PostAsJsonAsync(
      "/api/auth/login",
      new LoginUserRequest(email, Password));

    loginResponse.EnsureSuccessStatusCode();

    var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginUserResponse>();

    Assert.NotNull(loginResult);

    await factory.DeactivateUserAsync(email);

    var refreshResponse = await client.PostAsJsonAsync(
      "/api/auth/refresh",
      new RefreshTokenRequest(loginResult.RefreshToken));

    Assert.Equal(HttpStatusCode.Unauthorized, refreshResponse.StatusCode);
  }

  private HttpClient CreateClient()
  {
    return factory.CreateClient(
      new WebApplicationFactoryClientOptions
      {
        BaseAddress = new Uri("https://localhost")
      });
  }

  private static async Task RegisterUserAsync(HttpClient client, string email)
  {
    var response = await client.PostAsJsonAsync(
      "/api/auth/register", 
      new { name = "Inactive User", email, password = Password });

    response.EnsureSuccessStatusCode();
  }

  private static string CreateUniqueEmail()
  {
    return $"inactive-{Guid.NewGuid():N}@example.com";
  }
}
