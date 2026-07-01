using System.Security.Cryptography;
using System.Text;
using AuthSystem.Application.Abstractions.Security;

namespace AuthSystem.Infrastructure.Security;

public sealed class RefreshTokenHasher : IRefreshTokenHasher
{
  public string Hash(string token)
  {
    if (string.IsNullOrWhiteSpace(token))
    {
      throw new ArgumentException("Refresh token is required.", nameof(token));
    }

    var tokenBytes = Encoding.UTF8.GetBytes(token);
    var hashBytes = SHA256.HashData(tokenBytes);

    return Convert.ToHexString(hashBytes);
  }  
}