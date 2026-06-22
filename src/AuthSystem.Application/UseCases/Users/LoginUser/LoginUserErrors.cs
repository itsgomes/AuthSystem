using AuthSystem.Application.Common;

namespace AuthSystem.Application.UseCases.Users.LoginUser;

public static class LoginUserErrors
{
  public static readonly Error EmailRequired = 
    new("Users.EmailRequired", "Email is required");

  public static readonly Error PasswordRequired = 
    new("Users.PasswordRequired", "Password is required.");

  public static readonly Error InvalidCredentials = 
    new("Users.InvalidCredentials", "Invalid email or password.");
}