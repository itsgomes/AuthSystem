using AuthSystem.Application.Abstractions.Persistence;
using AuthSystem.Domain.Entities;
using AuthSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuthSystem.Infrastructure.Repositories;

public class PermissionRepository(AppDbContext context) : IPermissionRepository
{
  private readonly AppDbContext _context = context;

  public async Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    return await _context.Permissions
      .SingleOrDefaultAsync(permission => permission.Id == id, cancellationToken);
  }

  public async Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
  {
    return await _context.Permissions
      .SingleOrDefaultAsync(permission => permission.Name == name, cancellationToken);
  }

  public async Task<IReadOnlyList<string>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
  {
    return await _context.UserRoles
      .Where(userRole => userRole.UserId == userId)
      .SelectMany(userRole => userRole.Role.RolePermissions)
      .Select(rolePermission => rolePermission.Permission.Name)
      .Distinct()
      .ToListAsync(cancellationToken);
  }

  public async Task AddAsync(Permission permission, CancellationToken cancellationToken = default)
  {
    await _context.Permissions
      .AddAsync(permission, cancellationToken);
  }
}