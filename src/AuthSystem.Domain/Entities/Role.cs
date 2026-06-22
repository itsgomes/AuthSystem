namespace AuthSystem.Domain.Entities;

public class Role : Entity
{
	public string Name { get; private set; } = string.Empty;
	public ICollection<UserRole> UserRoles { get; private set; } = [];
	public ICollection<RolePermission> RolePermissions { get; private set; } = [];

	private Role()
	{
	}

	public Role(string name)
	{
		Id = Guid.NewGuid();
		Name = name;

		UserRoles = [];
		RolePermissions = [];
	}
}