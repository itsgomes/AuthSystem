namespace AuthSystem.Domain.Entities;

public class User : Entity
{
	public string Name { get; private set; } = string.Empty;
	public string Email { get; private set; } = string.Empty;
	public string PasswordHash { get; private set; } = string.Empty;
	public bool Active { get; private set; }
	public DateTime CreatedAt { get; private set; }
	public ICollection<UserRole> UserRoles { get; private set; } = [];
	public ICollection<RefreshToken> RefreshTokens { get; private set; } = [];

	private User()
	{
	}

	public User(string name, string email, string passwordHash)
	{
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Name is required.");

		if (name.Length < 3)
			throw new ArgumentException("Name must be at least 3 characters long.");

		if (string.IsNullOrWhiteSpace(email))
			throw new ArgumentException("Email is required.");

		if (!email.Contains('@'))
			throw new ArgumentException("Email is invalid.");

		if (string.IsNullOrWhiteSpace(passwordHash))
			throw new ArgumentException("Password is required.");

		Id = Guid.NewGuid();

		Name = name;
		Email = email;
		PasswordHash = passwordHash;

		Active = true;
		CreatedAt = DateTime.UtcNow;

		UserRoles = [];
		RefreshTokens = [];
	}
}