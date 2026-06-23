namespace AuthSystem.Application.Abstractions.Security;

public interface IRefreshTokenGenerator
{
  string Generate();
}