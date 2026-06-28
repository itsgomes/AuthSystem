using System.Net;
using System.Net.Http.Json;
using AuthSystem.Api.IntegrationTests.Infrastructure;
using AuthSystem.Application.UseCases.Users.LoginUser;
using AuthSystem.Application.UseCases.Users.LogoutUser;
using AuthSystem.Application.UseCases.Users.RefreshToken;
using Microsoft.AspNetCore.Mvc.Testing;
using RefreshTokenEntity = AuthSystem.Domain.Entities.RefreshToken;

namespace AuthSystem.Api.IntegrationTests.Authentication;

[Collection(PostgreSqlCollection.Name)]
public sealed class RefreshTokenLifecycleTests(PostgreSqlApiFactory factory)
{
  private const string Password = "Password@123";

  [Fact]
  public async Task Refresh_WithActiveToken_RotatesToken()
  {
    using var client = CreateClient();

    var login = await RegisterAndLoginAsync(client);

    var response = await client.PostAsJsonAsync(
      "/api/auth/refresh",
      new RefreshTokenRequest(login.RefreshToken));

    response.EnsureSuccessStatusCode();

    var result = await response.Content
      .ReadFromJsonAsync<RefreshTokenResponse>();

    Assert.NotNull(result);
    Assert.NotEqual(login.RefreshToken, result.RefreshToken);

    var originalToken = await factory.GetRefreshTokenAsync(login.RefreshToken);
    var replacementToken = await factory.GetRefreshTokenAsync(result.RefreshToken);

    Assert.NotNull(originalToken);
    Assert.NotNull(replacementToken);

    Assert.Equal(RefreshTokenEntity.RotatedReason, originalToken.RevokedReason);
    Assert.Equal(replacementToken.Id, originalToken.ReplacedByTokenId);

    Assert.True(replacementToken.IsActive);
  }

  [Fact]
  public async Task Reuse_OfRotatedToken_RevokesReplacement()
  {
    using var client = CreateClient();

    var login = await RegisterAndLoginAsync(client);

    var firstRefreshResponse = await client.PostAsJsonAsync(
      "/api/auth/refresh",
      new RefreshTokenRequest(login.RefreshToken));

    firstRefreshResponse.EnsureSuccessStatusCode();

    var firstRefresh = await firstRefreshResponse.Content
      .ReadFromJsonAsync<RefreshTokenResponse>();

    Assert.NotNull(firstRefresh);

    var reuseResponse = await client.PostAsJsonAsync(
      "/api/auth/refresh",
      new RefreshTokenRequest(login.RefreshToken));

    Assert.Equal(HttpStatusCode.Unauthorized, reuseResponse.StatusCode);

    var replacementToken = await factory.GetRefreshTokenAsync(firstRefresh.RefreshToken);
    
    Assert.NotNull(replacementToken);
    
    Assert.Equal(RefreshTokenEntity.ReuseDetectedReason, replacementToken.RevokedReason);
    
    Assert.False(replacementToken.IsActive);
  }

  [Fact]
  public async Task Logout_RevokesRefreshToken()
  {
    using var client = CreateClient();

    var login = await RegisterAndLoginAsync(client);

    var logoutResponse = await client.PostAsJsonAsync(
      "/api/auth/logout",
      new LogoutUserRequest(login.RefreshToken));

    Assert.Equal(HttpStatusCode.NoContent, logoutResponse.StatusCode);

    var refreshResponse = await client.PostAsJsonAsync(
      "/api/auth/refresh",
      new RefreshTokenRequest(login.RefreshToken));

    Assert.Equal(HttpStatusCode.Unauthorized, refreshResponse.StatusCode);

    var refreshToken = await factory.GetRefreshTokenAsync(login.RefreshToken);

    Assert.NotNull(refreshToken);

    Assert.Equal(RefreshTokenEntity.LogoutReason, refreshToken.RevokedReason);

    Assert.False(refreshToken.IsActive);
  }

  public HttpClient CreateClient()
  {
    return factory.CreateClient(
      new WebApplicationFactoryClientOptions
      {
        BaseAddress = new Uri("https://localhost")
      });
  }

  private static async Task<LoginUserResponse> RegisterAndLoginAsync(HttpClient client)
  {
    var email = $"refresh-{Guid.NewGuid():N}@example.com";

    var registerResponse = await client.PostAsJsonAsync(
      "/api/auth/register",
      new
      {
        name = "Refresh User",
        email,
        password = Password
      });

    registerResponse.EnsureSuccessStatusCode();

    var loginResponse = await client.PostAsJsonAsync(
      "/api/auth/login",
      new LoginUserRequest(email, Password));

    loginResponse.EnsureSuccessStatusCode();

    var login = await loginResponse.Content
      .ReadFromJsonAsync<LoginUserResponse>();

    Assert.NotNull(login);

    return login;
  }
}