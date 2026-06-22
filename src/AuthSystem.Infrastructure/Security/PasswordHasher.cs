using AuthSystem.Application.Abstractions.Security;
using Microsoft.AspNetCore.Identity;

namespace AuthSystem.Infrastructure.Security;

public sealed class PasswordHasher : IPasswordHasher
{
  private readonly PasswordHasher<object> _passwordHasher = new();

  public string Hash(string password)
  {
    return _passwordHasher.HashPassword(new object(), password);
  }

  public bool Verify(string password, string passwordHash)
  {
    var result = _passwordHasher.VerifyHashedPassword(
      new object(),
      passwordHash,
      password);

    return result == PasswordVerificationResult.Success ||
      result == PasswordVerificationResult.SuccessRehashNeeded;
  }
}