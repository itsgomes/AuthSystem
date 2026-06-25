using AuthSystem.Domain.Entities;

namespace AuthSystem.Application.Abstractions.Persistence;

public interface IRefreshTokenRepository
{
	Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);

	Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

	Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
}
