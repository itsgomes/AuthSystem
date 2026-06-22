using AuthSystem.Domain.Entities;

namespace AuthSystem.Application.Abstractions.Persistence;

public interface IPermissionRepository
{
	Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

	Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

	Task<IReadOnlyList<Permission>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

	Task AddAsync(Permission permission, CancellationToken cancellationToken = default);
}
