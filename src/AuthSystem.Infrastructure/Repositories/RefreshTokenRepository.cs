using AuthSystem.Application.Abstractions.Persistence;
using AuthSystem.Domain.Entities;
using AuthSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuthSystem.Infrastructure.Repositories;

public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
  private readonly AppDbContext _context = context;

  public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
  {
    return await _context.RefreshTokens
      .Include(refreshToken => refreshToken.User)
      .FirstOrDefaultAsync(refreshToken => refreshToken.Token == token, cancellationToken);
  }

  public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
  {
    await _context.RefreshTokens
      .AddAsync(refreshToken, cancellationToken);
  }
}