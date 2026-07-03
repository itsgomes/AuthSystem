using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AuthSystem.Api.IntegrationTests.Infrastructure;
using AuthSystem.Application.Authorization;
using AuthSystem.Application.UseCases.Users.LoginUser;
using AuthSystem.Application.UseCases.Users.RegisterUser;
using Microsoft.AspNetCore.Mvc.Testing;
using RefreshTokenEntity = AuthSystem.Domain.Entities.RefreshToken;

namespace AuthSystem.Api.IntegrationTests.Authorization;

[Collection(PostgreSqlCollection.Name)]
public sealed class UserDeactivationTests(PostgreSqlApiFactory factory)
{
  private const string Password = "Password@123";

  [Fact]
  public async Task Deactivate_WithoutAccessToken_Returns401()
  {
    using var client = CreateClient();

    var target = await RegisterUserAsync(client);

    var response = await DeactivateAsync(client, target.Id);

    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
  }

  [Fact]
  public async Task Deactivate_WithoutPermission_Returns403()
  {
    using var client = CreateClient();

    var target = await RegisterUserAsync(client);

    var accessToken = TestJwtGenerator.Generate();

    AddBearerToken(client, accessToken);

    var response = await DeactivateAsync(client, target.Id);

    Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
  }

  [Fact]
  public async Task Deactivate_WithUnknownUser_Returns404()
  {
    using var client = CreateClient();

    var accessToken = TestJwtGenerator.Generate(Permissions.UsersDeactivate);

    AddBearerToken(client, accessToken);

    var response = await DeactivateAsync(client, Guid.NewGuid());

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }

  [Fact]
  public async Task Deactivate_WithPermission_RevokesAllSessions()
  {
    using var client = CreateClient();

    var target = await RegisterUserAsync(client);

    var firstLogin = await LoginAsync(client, target);
    var secondLogin = await LoginAsync(client, target);

    var adminAccessToken = TestJwtGenerator.Generate(Permissions.UsersDeactivate);

    AddBearerToken(client, adminAccessToken);

    var response = await DeactivateAsync(client, target.Id);

    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

    var persistedUser = await factory.GetUserAsync(target.Id);

    Assert.NotNull(persistedUser);

    Assert.False(persistedUser.Active);

    var refreshTokens = await factory.GetRefreshTokensByUserIdAsync(target.Id);

    Assert.Equal(2, refreshTokens.Count);

    Assert.All(refreshTokens, refreshToken =>
    {
      Assert.False(refreshToken.IsActive);

      Assert.Equal(RefreshTokenEntity.UserDeactivatedReason, refreshToken.RevokedReason);
    });

    await factory.ActivateUserAsync(target.Id);

    var firstRefreshResponse = await client.PostAsJsonAsync(
      "/api/auth/refresh",
      new
      {
        refreshToken = firstLogin.RefreshToken
      });

    var secondRefreshResponse = await client.PostAsJsonAsync(
      "/api/auth/refresh",
      new
      {
        refreshToken = secondLogin.RefreshToken
      });

    Assert.Equal(HttpStatusCode.Unauthorized, firstRefreshResponse.StatusCode);
    Assert.Equal(HttpStatusCode.Unauthorized, secondRefreshResponse.StatusCode);
  }

  private HttpClient CreateClient()
  {
    return factory.CreateClient(
      new WebApplicationFactoryClientOptions
      {
        BaseAddress = new Uri("https://localhost")
      });
  }

  private static void AddBearerToken(HttpClient client, string token)
  {
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
  }

  private static async Task<HttpResponseMessage> DeactivateAsync(HttpClient client, Guid userId)
  {
    var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/users/{userId}/deactivate");

    return await client.SendAsync(request);
  }

  private static async Task<RegisterUserResponse> RegisterUserAsync(HttpClient client)
  {
    var email = $"deactivate-{Guid.NewGuid():N}@example.com";

    var response = await client.PostAsJsonAsync(
      "/api/auth/register",
      new
      {
        name = "Target User",
        email,
        password = Password
      });

    response.EnsureSuccessStatusCode();

    var user = await response.Content
      .ReadFromJsonAsync<RegisterUserResponse>();

    Assert.NotNull(user);

    return user;
  }

  private static async Task<LoginUserResponse> LoginAsync(HttpClient client, RegisterUserResponse user)
  {
    var response = await client.PostAsJsonAsync(
      "/api/auth/login",
      new LoginUserRequest(user.Email, Password));

    response.EnsureSuccessStatusCode();

    var login = await response.Content
      .ReadFromJsonAsync<LoginUserResponse>();

    Assert.NotNull(login);

    return login;
  }
}