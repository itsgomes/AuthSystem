using AuthSystem.Application.Common;

namespace AuthSystem.Application.UseCases.Users.RegisterUser;

public static class RegisterUserErrors
{
  public static readonly Error NameRequired = 
    new("Users.NameRequired", "Name is required.");

  public static readonly Error EmailRequired = 
    new("Users.EmailRequired", "Email is required.");

  public static readonly Error PasswordRequired = 
    new("Users.PasswordRequired", "Password is required.");

  public static readonly Error PasswordTooShort = 
    new("Users.PasswordTooShort", "Password must be at least 8 characters long.");

  public static readonly Error EmailAlreadyExists = 
    new("Users.EmailAlreadyExists", "Email is already in use.");
}