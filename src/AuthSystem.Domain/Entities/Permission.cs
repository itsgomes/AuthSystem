namespace AuthSystem.Domain.Entities;

public class Permission : Entity
{
	public string Name { get; private set; } = string.Empty;
	public ICollection<RolePermission> RolePermissions { get; private set; } = [];

	private Permission()
	{
	}

	public Permission(string name)
	{
		Id = Guid.NewGuid();
		Name = name;

		RolePermissions = [];
	}
}