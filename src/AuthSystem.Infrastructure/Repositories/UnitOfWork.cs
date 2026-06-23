using AuthSystem.Application.Abstractions.Persistence;
using AuthSystem.Application.Common;
using AuthSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuthSystem.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
  private readonly AppDbContext _context = context;

  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      return await _context.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateConcurrencyException ex)
    {
      throw new ConcurrencyException("A concurrency conflict occurred while saving changes.", ex);
    }
  }
}