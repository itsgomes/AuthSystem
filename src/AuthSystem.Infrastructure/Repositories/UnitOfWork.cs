using AuthSystem.Application.Abstractions.Persistence;
using AuthSystem.Infrastructure.Persistence;

namespace AuthSystem.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
  private readonly AppDbContext _context = context;

  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    return await _context.SaveChangesAsync(cancellationToken);
  }
}