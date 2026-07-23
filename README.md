# ECommerceRoute

ASP.NET Core e-commerce API with JWT authentication, email verification (teaching NoOp), refresh tokens, catalog, basket, and user addresses.

Built with a clean layered architecture and **two EF Core DbContexts** sharing one SQL Server database (Identity vs business data).

## Solution structure

| Project | Role |
|---------|------|
| `ECommerce.API` | Minimal APIs, Swagger, DI composition |
| `ECommerce.UseCases` | Commands / queries, ports, settings |
| `ECommerce.Domain` | Entities, errors, `Result` pattern |
| `ECommerce.Infrastructure` | EF Core, Identity, JWT, cache, seeding |
| `tests/` | UseCases + architecture tests |

Lecture notes (Identity + JWT): [`docs/identity-module.html`](docs/identity-module.html)

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB / full instance) — default connection: `Server=.;Database=Stores;...`
- Optional: Redis for distributed HybridCache (`ConnectionStrings:redis`)

## Quick start

```bash
dotnet restore
dotnet run --project src/ECommerce.API
```

In Development the API will:

1. Apply Identity + Application migrations  
2. Seed roles, super-admin, brands/types  
3. Open Swagger UI  

Swagger: `https://localhost:<port>/swagger`

### Seeded super-admin

Configured in `appsettings.Development.json` under `Seed:SuperAdmin` (change the password for anything beyond local use).

## Authentication flow

1. **Register** — `POST /api/v1/auth/register`  
   Creates an unconfirmed user and stores a verification code (email is **not** sent yet).
2. **Copy the code from the console** — `NoOpEmailSender` logs it (student task: wire FluentEmail).
3. **Confirm email** — `POST /api/v1/auth/confirm-email` → access + refresh tokens.
4. **Login** — `POST /api/v1/auth/login` (confirmed accounts only).
5. **Refresh** — `POST /api/v1/auth/refresh` (rotates refresh token).
6. **Logout** — `POST /api/v1/auth/logout` (revokes refresh token).

Use the access token as:

```http
Authorization: Bearer {accessToken}
```

### Basket buyer id

- **Guest:** send header `X-Buyer-Id: {client-generated-guid}`
- **Authenticated:** buyer id comes from the JWT (`NameIdentifier`); do not trust a spoofed header

After login, merge the guest basket with `POST /api/v1/basket/merge`.

## Main API routes (v1)

| Area | Examples |
|------|----------|
| Auth | `/api/v1/auth/register`, `confirm-email`, `login`, `refresh`, `logout` |
| Users | `/api/v1/users/me`, `/api/v1/users/me/addresses` |
| Catalog | products, brands, types |
| Basket | `/api/v1/basket`, items, merge |

## Architecture notes

- **Option B:** one SQL database, two contexts  
  - `AppIdentityDbContext` → Identity tables + refresh tokens (`identity.__IdentityMigrationsHistory`)  
  - `ApplicationDbContext` → products, brands, types, addresses (`app.__ApplicationMigrationsHistory`)
- UseCases return `Result` / `Result<T>` (no exception-driven flow for expected failures).
- JWT access tokens are short-lived; refresh tokens are opaque, hashed in the DB, and rotated on use.

## Configuration

Important sections in `appsettings` / `appsettings.Development.json`:

- `ConnectionStrings:DefaultConnection`
- `Jwt` — `Secret` (≥ 32 chars), issuer, audience, access/refresh lifetimes
- `EmailVerification` — code length, expiry, max attempts
- `Email` — reserved for FluentEmail (student task)
- `Seed:SuperAdmin`
- `CachedAggregates:Basket`
- `CloudinarySettings` — product image uploads (use your own keys)

## Migrations

```bash
# Identity
dotnet ef migrations add <Name> --context AppIdentityDbContext --output-dir Migrations/Identity --project src/ECommerce.Infrastructure --startup-project src/ECommerce.API

# Application
dotnet ef migrations add <Name> --context ApplicationDbContext --output-dir Migrations/Application --project src/ECommerce.Infrastructure --startup-project src/ECommerce.API
```

## Student task — email

`IEmailSender` is registered as `NoOpEmailSender` so verification codes are logged to the console (and stored in HybridCache).

Replace it with a FluentEmail SMTP implementation and wire `AddFluentEmail` in Infrastructure DI. Packages `FluentEmail.Core` / `FluentEmail.Smtp` are already referenced.

## Tests

```bash
dotnet test
```

## License

Private / course material unless otherwise stated.
