# AuthSystem

Web API didática de autenticação e autorização construída com ASP.NET Core e organizada segundo os princípios da Clean Architecture.

O projeto demonstra um fluxo completo de autenticação com JWT, renovação segura de sessões e autorização baseada em permissões, mantendo as regras de negócio separadas dos detalhes de HTTP, persistência e infraestrutura.

## Tecnologias

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core 10
- PostgreSQL 17
- Npgsql
- JWT Bearer Authentication
- Docker e Docker Compose
- xUnit
- `WebApplicationFactory`
- Testcontainers for .NET

## Funcionalidades

- Cadastro e login de usuários
- Hash seguro de senhas
- Access tokens JWT de curta duração
- Refresh tokens com rotação
- Revogação do token anterior durante a rotação
- Detecção de reutilização de refresh tokens
- Revogação de sessões após detecção de reuse
- Logout com revogação do refresh token
- Controle de concorrência otimista no refresh
- Bloqueio de login e refresh para usuários inativos
- Roles e permissions persistidas no PostgreSQL
- Permissions incluídas como claims no JWT
- Policies de autorização baseadas em permissions
- Testes de integração para autenticação e autorização
- Testes com uma instância descartável do PostgreSQL

## Arquitetura

```text
src/
  AuthSystem.Api/             # Controllers, autenticação, autorização e configuração HTTP
  AuthSystem.Application/     # Casos de uso, contratos, resultados e abstrações
  AuthSystem.Domain/          # Entidades e regras de domínio
  AuthSystem.Infrastructure/  # EF Core, PostgreSQL, repositories e serviços de segurança

tests/
  AuthSystem.Api.IntegrationTests/  # Testes da API e infraestrutura de testes
```

As dependências seguem em direção ao domínio: a API compõe a aplicação, a Application define os contratos e a Infrastructure implementa os detalhes externos.

## Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0)
- Docker com o Docker Engine em execução
- Docker Compose
- Ferramenta `dotnet-ef` 10, para aplicar as migrations

Caso ainda não tenha o `dotnet-ef`:

```bash
dotnet tool install --global dotnet-ef --version 10.0.9
```

## Configuração local

Clone o repositório e entre no diretório da solução:

```bash
git clone <url-do-repositorio>
cd AuthSystem
```

Configure a chave usada para assinar os JWTs por meio de User Secrets:

```bash
dotnet user-secrets set "Jwt:SecretKey" "uma-chave-segura-com-pelo-menos-32-caracteres" --project src/AuthSystem.Api
```

O issuer, a audience e a duração do access token estão em `src/AuthSystem.Api/appsettings.json`. A chave secreta não deve ser adicionada a esse arquivo nem versionada.

## Executando a aplicação

### 1. Inicie o PostgreSQL

```bash
docker compose up -d
```

O ambiente local utiliza:

```text
Host: localhost
Port: 5432
Database: authdb
Username: postgres
Password: postgres
```

Essas credenciais são destinadas somente ao desenvolvimento local.

### 2. Restaure as dependências

```bash
dotnet restore
```

### 3. Aplique as migrations

```bash
dotnet ef database update --project src/AuthSystem.Infrastructure --startup-project src/AuthSystem.Api
```

### 4. Inicie a API

```bash
dotnet run --project src/AuthSystem.Api
```

Por padrão, a aplicação fica disponível em:

```text
http://localhost:5181
https://localhost:7190
```

Em ambiente de desenvolvimento, o documento OpenAPI pode ser acessado em:

```text
http://localhost:5181/openapi/v1.json
```

## Executando os testes

Os testes exigem que o Docker Engine esteja em execução. Não é necessário iniciar o banco do `docker-compose.yml`: o Testcontainers cria uma instância isolada e descartável do PostgreSQL automaticamente.

Execute toda a suíte a partir da raiz do repositório:

```bash
dotnet test
```

Na primeira execução, o Docker poderá precisar baixar a imagem do PostgreSQL. Ao final dos testes, o container temporário é removido.

Os cenários cobertos incluem:

- Requisições sem access token
- Usuários autenticados sem a permission necessária
- Acesso com a permission correta
- Access tokens expirados
- Tokens assinados com uma chave desconhecida
- Login de usuário inativo
- Refresh token de usuário inativo

## Comandos úteis

Compilar a solução:

```bash
dotnet build
```

Parar o PostgreSQL local:

```bash
docker compose down
```

Parar o PostgreSQL e remover seu volume local:

```bash
docker compose down -v
```

Listar as migrations:

```bash
dotnet ef migrations list --project src/AuthSystem.Infrastructure --startup-project src/AuthSystem.Api
```

## Observações de segurança

- Nunca versione a `Jwt:SecretKey`.
- Use uma chave forte e mantenha secrets fora dos arquivos de configuração versionados.
- As credenciais do Docker Compose são próprias para desenvolvimento local.
- Em produção, utilize um gerenciador de segredos e credenciais diferentes para o banco.
- Access tokens são stateless: um token já emitido continua válido até sua expiração, mesmo que o usuário seja desativado posteriormente.
