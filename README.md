# Store API

A RESTful e-commerce backend built with **ASP.NET Core** and **Entity Framework Core**. Supports product/category/variant management, image handling, order processing with inventory control, and JWT-based admin authentication.

## Tech Stack

- **ASP.NET Core**
- **Entity Framework Core** with SQLite
- **JWT Bearer Authentication** for admin-only routes
- **xUnit** for integration testing

## Features

- **Product catalog** — products, categories, variants (SKU/size/color/stock), and images, each with full CRUD and pagination for products
- **Order processing** — creates orders from a cart of variant/quantity pairs, validates stock, and calculates totals server-side from the current product price
- **Concurrency-safe inventory** — stock is decremented with a single conditional inside a transaction, so two simultaneous orders can't oversell the same item; if stock is insufficient the transaction rolls back and the order fails with `409 Conflict`
- **Order lifecycle** — pending → paid/cancelled status updates, with cancellation logic scaffolded for a future Stripe integration
- **JWT authentication** — admin login issues a signed JWT; write operations (create/update/delete) are protected with `RequireAuthorization()`, while browsing endpoints stay public
- **Centralized error handling** — a global `IExceptionHandler` catches unhandled exceptions and returns consistent `ProblemDetails` JSON
- **Integration tests** — tests for API endpoints that ensure authorization, validation, and proper database row creation and responses

## API Overview

| Resource | Endpoints |
|---|---|
| `POST /auth/login` | Admin login, returns JWT |
| `GET /products` | Paginated list, filterable by category |
| `POST /products`, `PUT /products/{id}`, `DELETE /products/{id}` | Admin-only |
| `GET/POST/PUT/DELETE /products/{id}/variants` | Manage SKUs, size/color, stock |
| `GET/POST/PUT/DELETE /products/{id}/images` | Manage product images |
| `GET/POST /categories`, `PUT/DELETE /categories/{id}` | Category management |
| `GET /orders`, `GET /orders/{id}` | Paginated order lookup |
| `POST /orders` | Place an order (stock-checked, transactional) |
| `PATCH /orders/{id}/status` | Update order status (admin) |
| `PATCH /orders/{id}/cancel` | Cancel an order (admin) |


## Packages Used

Target framework: **.NET 10** (`net10.0`), with `Nullable` and `ImplicitUsings` enabled.

| Package | Version | Purpose |
|---|---|---|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 10.0.10 | JWT bearer auth middleware |
| `Microsoft.EntityFrameworkCore.Sqlite` | 10.0.10 | SQLite database provider |
| `Microsoft.EntityFrameworkCore.Design` | 10.0.10 | Enables `dotnet ef` migration tooling |

## Basic Flow: Building This Project From Scratch

1. **Scaffold the project**
```bash
   dotnet new web -n StoreApi.Api
   cd StoreApi.Api
   dotnet new xunit -n StoreApi.Tests -o ../StoreApi.Tests
```

2. **Add packages** — only three explicit references are needed; `Microsoft.AspNetCore.Identity` and the JWT handler come along via the shared framework / transitively
```bash
   dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 10.0.10
   dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 10.0.10
   dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.10
```

3. **Define models** — plain C# classes for `Product`, `Category`, `ProductVariant`, `ProductImage`, `Order`, `OrderItem`, `Admin`

4. **Create the `DbContext`** — expose each model as a `DbSet<T>`, register it in `Program.cs` with `AddDbContext<StoreContext>(...)`

5. **Generate and apply migrations**
```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
```

6. **Write DTOs** — request/response shapes per resource, so entities are never serialized directly

7. **Group and map endpoints** — one static class per resource (e.g. `ProductEndpoints`), each exposing a `MapXEndpoints(this IEndpointRouteBuilder app)` extension method, wired up in `Program.cs` via `app.MapGroup("/products")`

8. **Add business logic inside handlers** — querying, DTO mapping, transactional writes where needed (e.g. the order/stock logic)

9. **Add validation** — guard clauses or a validation library, checked before any DB write

10. **Add global exception handling** — implement `IExceptionHandler`, register with `AddExceptionHandler<T>()` + `AddProblemDetails()`, call `app.UseExceptionHandler()`

11. **Add authentication/authorization**
    - Register JWT auth in `Program.cs` (`AddAuthentication().AddJwtBearer(...)`)
    - Build a token generator service
    - Add a `/auth/login` endpoint
    - Apply `.RequireAuthorization()` to routes that should be admin-only
    - Call `app.UseAuthentication()` and `app.UseAuthorization()` in the pipeline (in that order)

12. **Seed initial data** — e.g. an admin user, on startup

13. **Write integration tests** — `WebApplicationFactory` to spin up the app in-memory, hit real endpoints, and assert both success and auth-failure cases

## Getting Started

```bash
git clone https://github.com/JoLe2004/ecommerce.git
cd store-api
dotnet restore
dotnet ef database update
dotnet run
```

Run the test suite:

```bash
dotnet test
```

## Project Structure

```
StoreApi.Api/
├── Endpoints/       # Minimal API route groups (Products, Orders, Auth, etc.)
├── Models/          # EF Core entities
├── Dtos/            # Request/response contracts
├── Data/            # DbContext and seeding
├── Auth/            # JWT token generation
└── ExceptionHandling/  # Global error handler
StoreApi.Tests/       # xUnit integration tests
```

## Skills Demonstrated

- Designing RESTful APIs with clear resource boundaries and consistent DTO shaping
- Using EF Core transactions and atomic updates to prevent race conditions
- Implementing token-based authentication and role-gating endpoints
- Writing integration tests that exercise the full HTTP pipeline, including auth failures
- Structuring a multi-file ASP.NET Core project with separation of concerns (endpoints, data access, DTOs, auth, error handling)

## Possible Next Steps

- Stripe integration for payments and refunds
- OpenAPI documentation
- Docker Compose setup for local development