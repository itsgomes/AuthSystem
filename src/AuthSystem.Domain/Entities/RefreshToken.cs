namespace AuthSystem.Domain.Entities;

public class RefreshToken
{
	public const string RotatedReason = "Rotated";
	public const string ReuseDetectedReason = "ReuseDetected";
	public const string LogoutReason = "Logout";

	public Guid Id { get; private set; }
	public string TokenHash { get; private set; } = string.Empty;
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
	public bool WasRevokedByRotation => IsRevoked && RevokedReason == RotatedReason && ReplacedByTokenId is not null;

	private RefreshToken()
	{
	}

	public RefreshToken(string tokenHash, DateTime expiresAt, Guid userId)
	{
		if (string.IsNullOrWhiteSpace(tokenHash))
			throw new ArgumentException("Refresh token hash is required.");

		if (expiresAt <= DateTime.UtcNow)
			throw new ArgumentException("Refresh token expiration must be in the future.");

		Id = Guid.NewGuid();
		
		TokenHash = tokenHash;
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

	public void RevokeDueToReuseDetected()
	{
		Revoke(ReuseDetectedReason);
	}

	public void RevokeDueToLogout()
	{
		Revoke(LogoutReason);
	}
}
