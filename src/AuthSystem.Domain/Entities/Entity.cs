using System;
using System.Collections.Generic;
using System.Text;

namespace AuthSystem.Domain.Entities;

public abstract class Entity
{
	public Guid Id { get; protected set; }
}
