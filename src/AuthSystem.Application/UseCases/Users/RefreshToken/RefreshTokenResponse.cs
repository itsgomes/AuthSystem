namespace AuthSystem.Application.UseCases.Users.RefreshToken;

public sealed record RefreshTokenResponse(
  string AccessToken,
  string RefreshToken);