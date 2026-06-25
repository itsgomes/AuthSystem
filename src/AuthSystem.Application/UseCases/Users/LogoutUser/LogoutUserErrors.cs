using AuthSystem.Application.Common;

namespace AuthSystem.Application.UseCases.Users.LogoutUser;

public static class LogoutUserErrors
{
  public static readonly Error RefreshTokenRequired = new(
    "Logout.RefreshTokenRequired",
    "Refresh token is required.");

  public static readonly Error InvalidRefreshToken = new(
    "Logout.InvalidRefreshToken",
    "Refresh token is invalid.");
}