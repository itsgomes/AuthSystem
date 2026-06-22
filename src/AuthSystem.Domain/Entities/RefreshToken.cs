namespace AuthSystem.Domain.Entities;

public class RefreshToken
{
	public Guid Id { get; private set; }
	public string Token { get; private set; } = string.Empty;
	public DateTime ExpiresAt { get; private set; }
	public bool Revoked { get; private set; }
	public Guid UserId { get; private set; }
	public User User { get; private set; } = null!;

	private RefreshToken()
	{
	}

	public RefreshToken(string token, DateTime expiresAt, Guid userId)
	{
		Id = Guid.NewGuid();

		Token = token;
		ExpiresAt = expiresAt;
		UserId = userId;

		Revoked = false;
	}
}