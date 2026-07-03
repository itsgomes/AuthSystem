# AuthSystem

Authentication and authorization Web API built with ASP.NET Core and Clean Architecture.

## Tech Stack

- .NET 10 and ASP.NET Core Web API
- Entity Framework Core and PostgreSQL 17
- JWT Bearer authentication
- Role- and permission-based authorization
- Docker and Docker Compose
- xUnit, `WebApplicationFactory`, and Testcontainers

## Requirements

- [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0)
- Docker with Docker Compose
- `dotnet-ef` 10

## Configuration

Set a JWT signing key with at least 32 characters:

```bash
dotnet user-secrets set "Jwt:SecretKey" "your-secure-key-with-at-least-32-characters" --project src/AuthSystem.Api
```

The development database connection and non-sensitive JWT settings are defined in `src/AuthSystem.Api/appsettings.json`.

## Run

Start PostgreSQL:

```bash
docker compose up -d
```

Restore dependencies and apply the migrations:

```bash
dotnet restore
dotnet ef database update --project src/AuthSystem.Infrastructure --startup-project src/AuthSystem.Api
```

Start the API:

```bash
dotnet run --project src/AuthSystem.Api
```

Development URLs:

```text
http://localhost:5181
https://localhost:7190
http://localhost:5181/openapi/v1.json
```

## Tests

Keep Docker running and execute:

```bash
dotnet test
```

Integration tests use Testcontainers to create an isolated, disposable PostgreSQL instance. The Docker Compose database does not need to be running for the tests.

## Notes

- Local PostgreSQL credentials in `docker-compose.yml` are intended for development only.
- Never commit `Jwt:SecretKey` or production credentials.
- Store production secrets in a dedicated secret manager.
- Refresh tokens are stored as SHA-256 hashes.
- Access tokens are stateless and remain valid until expiration.
