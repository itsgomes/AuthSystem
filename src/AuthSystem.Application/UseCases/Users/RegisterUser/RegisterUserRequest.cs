namespace AuthSystem.Application.UseCases.Users.RegisterUser;

public sealed record RegisterUserRequest(
  string Name,
  string Email,
  string Password);
