using AuthSystem.Application.Abstractions.Persistence;
using AuthSystem.Domain.Entities;
using AuthSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuthSystem.Infrastructure.Repositories;

public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
  private readonly AppDbContext _context = context;

  public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
  {
    return await _context.RefreshTokens
      .Include(refreshToken => refreshToken.User)
      .FirstOrDefaultAsync(refreshToken => refreshToken.TokenHash == tokenHash, cancellationToken);
  }

  public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
  {
    return await _context.RefreshTokens
      .Where(refreshToken =>
        refreshToken.UserId == userId &&
        refreshToken.RevokedAt == null &&
        refreshToken.ExpiresAt > DateTime.UtcNow)
      .ToListAsync(cancellationToken);
  }

  public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
  {
    await _context.RefreshTokens
      .AddAsync(refreshToken, cancellationToken);
  }
}