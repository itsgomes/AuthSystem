namespace AuthSystem.Application.UseCases.Users.LoginUser;

public sealed record LoginUserRequest(
  string Email,
  string Password);