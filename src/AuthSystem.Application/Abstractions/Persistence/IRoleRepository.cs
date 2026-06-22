using AuthSystem.Domain.Entities;

namespace AuthSystem.Application.Abstractions.Persistence;

public interface IRoleRepository
{
	Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

	Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

	Task AddAsync(Role role, CancellationToken cancellationToken = default);
}
