using System.Security.Cryptography;
using AuthSystem.Application.Abstractions.Security;

namespace AuthSystem.Infrastructure.Security;

public sealed class RefreshTokenGenerator : IRefreshTokenGenerator
{
  public string Generate()
  {
    var randomBytes = RandomNumberGenerator.GetBytes(64);

    return Convert.ToBase64String(randomBytes);
  }
}