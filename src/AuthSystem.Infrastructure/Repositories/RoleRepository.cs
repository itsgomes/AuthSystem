using AuthSystem.Application.Abstractions.Persistence;
using AuthSystem.Domain.Entities;
using AuthSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuthSystem.Infrastructure.Repositories;

public class RoleRepository(AppDbContext context) : IRoleRepository
{
  private readonly AppDbContext _context = context;

  public async Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    return await _context.Roles
      .SingleOrDefaultAsync(role => role.Id == id, cancellationToken);
  }

  public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
  {
    return await _context.Roles
      .SingleOrDefaultAsync(role => role.Name == name, cancellationToken);
  }

  public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
  {
    await _context.Roles
      .AddAsync(role, cancellationToken);
  }
}