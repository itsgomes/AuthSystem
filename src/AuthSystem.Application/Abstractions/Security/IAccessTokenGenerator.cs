using AuthSystem.Domain.Entities;

namespace AuthSystem.Application.Abstractions.Security;

public interface IAccessTokenGenerator
{
  string Generate(User user);
}