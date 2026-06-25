using AuthSystem.Application.Common;

namespace AuthSystem.Application.UseCases.Users.RefreshToken;

public static class RefreshTokenErrors
{
  public static readonly Error RefreshTokenRequired = new(
    "RefreshToken.Required",
    "Refresh token is required.");

  public static readonly Error InvalidRefreshToken = new(
    "RefreshToken.Invalid",
    "Refresh token is invalid.");

  public static readonly Error RefreshTokenReused = new(
    "RefreshToken.Reused",
    "Refresh token reuse detected.");
}