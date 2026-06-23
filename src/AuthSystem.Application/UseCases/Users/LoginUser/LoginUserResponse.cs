namespace AuthSystem.Application.UseCases.Users.LoginUser;

public sealed record LoginUserResponse(
  Guid Id,
  string Name,
  string Email,
  string AccessToken,
  string RefreshToken);