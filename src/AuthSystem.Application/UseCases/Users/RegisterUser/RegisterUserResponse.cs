namespace AuthSystem.Application.UseCases.Users.RegisterUser;

public sealed record RegisterUserResponse(
  Guid Id,
  string Name,
  string Email);
