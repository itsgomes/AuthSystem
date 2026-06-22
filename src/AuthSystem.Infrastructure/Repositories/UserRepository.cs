using AuthSystem.Application.Abstractions.Persistence;
using AuthSystem.Domain.Entities;
using AuthSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuthSystem.Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
  private readonly AppDbContext _context = context;

  public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    return await _context.Users
      .SingleOrDefaultAsync(user => user.Id == id, cancellationToken);
  }

  public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
  {
    return await _context.Users
      .SingleOrDefaultAsync(user => user.Email == email, cancellationToken);
  }

  public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
  {
    return await _context.Users
      .AnyAsync(user => user.Email == email, cancellationToken);
  }

  public async Task AddAsync(User user, CancellationToken cancellationToken = default)
  {
    await _context.Users
      .AddAsync(user, cancellationToken);
  }
}