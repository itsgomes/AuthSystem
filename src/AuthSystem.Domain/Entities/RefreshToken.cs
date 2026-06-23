namespace AuthSystem.Domain.Entities;

public class RefreshToken
{
	public Guid Id { get; private set; }
	public string Token { get; private set; } = string.Empty;
	public DateTime CreatedAt { get; private set; }
	public DateTime ExpiresAt { get; private set; }
	public DateTime? RevokedAt { get; private set; }
	public string? RevokedReason { get; private set; }
	public Guid? ReplacedByTokenId { get; private set; }
	public int Version { get; private set; }
	
	public Guid UserId { get; private set; }
	public User User { get; private set; } = null!;

	public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
	public bool IsRevoked => RevokedAt is not null;
	public bool IsActive => !IsRevoked && !IsExpired;

	private RefreshToken()
	{
	}

	public RefreshToken(string token, DateTime expiresAt, Guid userId)
	{
		if (string.IsNullOrWhiteSpace(token))
			throw new ArgumentException("Refresh token is required.");

		if (expiresAt <= DateTime.UtcNow)
			throw new ArgumentException("Refresh token expiration must be in the future.");

		Id = Guid.NewGuid();
		
		Token = token;
		CreatedAt = DateTime.UtcNow;
		ExpiresAt = expiresAt;
		UserId = userId;
	}

	public void Revoke(string revokedReason, Guid? replacedByTokenId = null)
	{
		if (IsRevoked)
			return;

		if (string.IsNullOrWhiteSpace(revokedReason))
			throw new ArgumentException("Revoked reason is required.");

		RevokedAt = DateTime.UtcNow;
		RevokedReason = revokedReason;
		ReplacedByTokenId = replacedByTokenId;
		Version++;
	}
}
