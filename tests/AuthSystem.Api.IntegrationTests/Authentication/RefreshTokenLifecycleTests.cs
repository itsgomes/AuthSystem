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

  [Fact]
  public async Task ConcurrentRefresh_WithSameToken_AllowsOnlyOneRotation()
  {
    using var firstClient = CreateClient();
    using var secondClient = CreateClient();

    var login = await RegisterAndLoginAsync(firstClient);

    var startGate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

    var request = new RefreshTokenRequest(login.RefreshToken);

    var firstTask = SendRefreshAfterSignalAsync(firstClient, request, startGate.Task);
    var secondTask = SendRefreshAfterSignalAsync(secondClient, request, startGate.Task);

    startGate.SetResult(true);

    var responses = await Task.WhenAll(firstTask, secondTask);

    using var firstResponse = responses[0];
    using var secondResponse = responses[1];

    var successfulResponses = responses
      .Where(response => response.StatusCode == HttpStatusCode.OK)
      .ToList();

    var unauthorizedResponses = responses
      .Where(response => response.StatusCode == HttpStatusCode.Unauthorized)
      .ToList();

    Assert.Single(successfulResponses);
    Assert.Single(unauthorizedResponses);

    var successfulRefresh = await successfulResponses[0]
      .Content
      .ReadFromJsonAsync<RefreshTokenResponse>();

    Assert.NotNull(successfulRefresh);

    var originalToken = await factory.GetRefreshTokenAsync(login.RefreshToken);

    Assert.NotNull(originalToken);

    var userTokens = await factory.GetRefreshTokensByUserIdAsync(originalToken.UserId);

    Assert.Equal(2, userTokens.Count);
    Assert.Equal(RefreshTokenEntity.RotatedReason, originalToken.RevokedReason);

    Assert.NotNull(originalToken.ReplacedByTokenId);

    var replacementToken = userTokens.Single(token => token.Id == originalToken.ReplacedByTokenId);

    Assert.Equal(successfulRefresh.RefreshToken, replacementToken.Token);
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

  private static async Task<HttpResponseMessage> SendRefreshAfterSignalAsync(HttpClient client, RefreshTokenRequest request, Task startSignal)
  {
    await startSignal;

    return await client.PostAsJsonAsync(
      "/api/auth/refresh", 
      request);
  }
}