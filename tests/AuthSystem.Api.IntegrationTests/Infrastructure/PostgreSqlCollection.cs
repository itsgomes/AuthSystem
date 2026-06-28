namespace AuthSystem.Api.IntegrationTests.Infrastructure;

[CollectionDefinition(Name)]
public sealed class PostgreSqlCollection : ICollectionFixture<PostgreSqlApiFactory>
{
  public const string Name = "PostgreSQL";
}