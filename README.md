# AuthSystem

Web API de autenticacao e autorizacao desenvolvida com ASP.NET Core, .NET 10, Entity Framework Core e PostgreSQL, seguindo uma organizacao inspirada em Clean Architecture.

## Tecnologias

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Docker Compose
- JWT Bearer Authentication
- User Secrets para configuracoes sensiveis em ambiente local

## Estrutura do projeto

```text
src/
  AuthSystem.Api/             # Camada de entrada HTTP, controllers e configuracao da API
  AuthSystem.Application/     # Casos de uso, contratos, resultados e abstracoes
  AuthSystem.Domain/          # Entidades de dominio
  AuthSystem.Infrastructure/  # EF Core, repositories, seguranca, migrations e PostgreSQL
```

## Funcionalidades

- Cadastro de usuario
- Login com geracao de access token e refresh token
- Renovacao de tokens
- Hash de senha
- Persistencia de usuarios, roles, permissoes e refresh tokens
- Autenticacao via JWT

## Pre-requisitos

- .NET SDK 10
- Docker
- Docker Compose

## Configuracao local

Clone o repositorio e acesse a pasta do projeto:

```bash
git clone <url-do-repositorio>
cd AuthSystem
```

Suba o PostgreSQL com Docker Compose:

```bash
docker compose up -d
```

O banco configurado no `docker-compose.yml` usa:

```text
Host: localhost
Port: 5432
Database: authdb
Username: postgres
Password: postgres
```

A connection string padrao já está em `src/AuthSystem.Api/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=authdb;Username=postgres;Password=postgres"
}
```

## User Secrets

A chave secreta do JWT nao deve ser versionada. Configure a `SecretKey` com User Secrets no projeto da API:

```bash
dotnet user-secrets set "Jwt:SecretKey" "sua-chave-com-no-minimo-32-caracteres" --project src/AuthSystem.Api
```

As demais configuracoes de JWT ficam em `appsettings.json`:

```json
"Jwt": {
  "Issuer": "AuthSystem",
  "Audience": "AuthSystem",
  "ExpirationInMinutes": 15
}
```

## Banco de dados

Para aplicar as migrations existentes:

```bash
dotnet ef database update --project src/AuthSystem.Infrastructure --startup-project src/AuthSystem.Api
```

Para criar uma nova migration:

```bash
dotnet ef migrations add NomeDaMigration --project src/AuthSystem.Infrastructure --startup-project src/AuthSystem.Api
```

## Executando a API

Restaure, compile e execute o projeto:

```bash
dotnet restore
dotnet build
dotnet run --project src/AuthSystem.Api
```

Em ambiente de desenvolvimento, a API sobe por padrao em:

```text
http://localhost:5181
https://localhost:7190
```

O documento OpenAPI fica disponivel em:

```text
http://localhost:5181/openapi/v1.json
```

## Endpoints

Base path:

```text
/api/auth
```

### Registrar usuario

```http
POST /api/auth/register
Content-Type: application/json
```

```json
{
  "name": "Rafael",
  "email": "rafael@example.com",
  "password": "Senha@123"
}
```

Resposta esperada:

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "name": "Rafael",
  "email": "rafael@example.com"
}
```

### Login

```http
POST /api/auth/login
Content-Type: application/json
```

```json
{
  "email": "rafael@example.com",
  "password": "Senha@123"
}
```

Resposta esperada:

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "name": "Rafael",
  "email": "rafael@example.com",
  "accessToken": "<jwt>",
  "refreshToken": "<refresh-token>"
}
```

### Renovar token

```http
POST /api/auth/refresh
Content-Type: application/json
```

```json
{
  "refreshToken": "<refresh-token>"
}
```

Resposta esperada:

```json
{
  "accessToken": "<jwt>",
  "refreshToken": "<refresh-token>"
}
```

### Logout

```http
POST /api/auth/logout
Content-Type: application/json
```

```json
{
  "refreshToken": "<refresh-token>"
}
```

Resposta esperada:
Status HTTP: 204

## Comandos uteis

Parar o banco:

```bash
docker compose down
```

Parar o banco e remover o volume local:

```bash
docker compose down -v
```

Verificar as migrations:

```bash
dotnet ef migrations list --project src/AuthSystem.Infrastructure --startup-project src/AuthSystem.Api
```

## Observacoes de seguranca

- Nunca versione a `Jwt:SecretKey`.
- Use uma chave forte, com no minimo 32 caracteres.
- Em producao, prefira armazenar segredos em um provedor seguro, como variaveis de ambiente, Azure Key Vault, AWS Secrets Manager ou equivalente.
- O `docker-compose.yml` atual é voltado para desenvolvimento local.
