# Copilot Instructions for BolaoApi

This document helps Copilot sessions work effectively on the BolaoApi project.

## Project Overview

**BolaoApi** is an ASP.NET Core 10.0 minimal API project written in C#. It uses a modern, lightweight architecture without traditional controllers.

## Build, Test, and Run

### Prerequisites
- .NET 10.0 SDK installed
- Visual Studio, JetBrains Rider, or VS Code with C# Dev Kit

### Build
```bash
dotnet build
```

### Run
```bash
# Development mode (HTTP on port 5010, HTTPS on port 7060)
dotnet run --project BolaoApi/BolaoApi.csproj

# Specific profile
dotnet run --project BolaoApi/BolaoApi.csproj --launch-profile https
```

### Available Profiles
- **http**: Runs on `http://localhost:5010` (no browser launch)
- **https**: Runs on `http://localhost:5010` and `https://localhost:7060`

### View API Documentation
- **OpenAPI/Swagger**: `http://localhost:5010/openapi/v1.json` (when running in Development)

### Test Endpoints
Use the `BolaoApi.http` file in the project root for testing endpoints:
```
GET http://localhost:5010/weatherforecast
```

## Architecture

### DDD (Domain-Driven Design)
The project follows DDD principles with clear separation of concerns:

**Folder Structure:**
- **Domain**: Core business logic and entities
  - `Entities/`: Domain models (e.g., `User`)
  - `Interfaces/`: Contracts for repositories
- **Application**: Use cases and application logic
  - `Dtos/`: Data transfer objects for API contracts
- **Infrastructure**: Data access and external services
  - `Data/`: Entity Framework DbContext
  - `Repositories/`: Repository implementations for domain entities
- **Program.cs**: Presentation layer - API endpoints

### Data Access
- **Entity Framework Core 10.0**: Code-First approach using SQLite
- **Database**: SQLite (`bolao.db`) - specified in appsettings.json
- **Migrations**: Automatic migrations on startup via `dbContext.Database.MigrateAsync()`

### Minimal API Pattern
- **No Controllers**: Endpoints defined directly in `Program.cs`
- **Dependency Injection**: DbContext and repositories injected at runtime
- **Repository Pattern**: `IUserRepository` abstracts data access

### Configuration Files
- **appsettings.json**: Database connection string and JWT settings
- **appsettings.Development.json**: Development environment overrides
- **launchSettings.json**: Launch profiles for running with different URLs

### Current Schema
**Users Table:**
- `Id` (TEXT PRIMARY KEY) - GUID
- `Name` (TEXT NOT NULL) - Max 255 chars
- `Email` (TEXT NOT NULL UNIQUE) - Max 255 chars, indexed
- `PasswordHash` (TEXT NOT NULL) - Bcrypt hash
- `CreatedAt` (TEXT NOT NULL) - Timestamp with default

## Key Conventions

### Endpoint Definition
Endpoints are added directly in `Program.cs` after `var app = app.Build()`:
```csharp
app.MapPost("/auth/register", async (RegisterRequestDto request, IUserRepository userRepository) =>
    {
        // handler
    })
    .WithName("Register")
    .WithOpenApi();
```
Repositories are injected as parameters for dependency injection.

### Domain Models
Located in `Domain/Entities/`. Create domain entities with business logic:
```csharp
public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
```

### DTOs
Located in `Application/Dtos/`. Use records for immutability:
```csharp
record RegisterRequestDto(string Name, string Email, string Password);
```

### Repositories
Located in `Infrastructure/Repositories/`. Implement `IUserRepository` interface:
```csharp
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    // implementation
}
```

### Database Operations
Always use async operations:
```csharp
var user = await userRepository.GetByEmailAsync(email);
await userRepository.SaveChangesAsync();
```

### Naming Conventions
- **Endpoints**: Use kebab-case routes (`/auth-register`)
- **Methods**: PascalCase for C# methods
- **Classes/Records**: PascalCase
- **Folders**: PascalCase (Domain, Application, Infrastructure)
- **DTOs**: Suffix with `Dto` (e.g., `RegisterRequestDto`)

### DbContext Configuration
Located in `Infrastructure/Data/`. Configure entity mappings in `OnModelCreating()`:
```csharp
modelBuilder.Entity<User>(entity =>
{
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Email).HasIndex().IsUnique();
});
```

## Nullable Reference Types

The project has **nullable reference types enabled** (`<Nullable>enable</Nullable>` in .csproj).
- Use `?` to mark nullable types: `string?`
- Non-nullable types must be properly initialized
- This catches potential null reference bugs at compile time

## Common Tasks

### Adding a New Endpoint
1. Create DTOs in `Application/Dtos/` (request/response)
2. Create domain entity in `Domain/Entities/` (if needed)
3. Create/update repository interface in `Domain/Interfaces/`
4. Implement repository in `Infrastructure/Repositories/`
5. Register repository in `Program.cs`: `builder.Services.AddScoped<IUserRepository, UserRepository>();`
6. Add endpoint mapping in `Program.cs` with dependency injection

### Adding a New Domain Entity
1. Create entity class in `Domain/Entities/`
2. Create repository interface in `Domain/Interfaces/`
3. Create repository implementation in `Infrastructure/Repositories/`
4. Add `DbSet<Entity>` to `ApplicationDbContext`
5. Configure entity in `OnModelCreating()` with constraints and indexes
6. Create migration: `dotnet ef migrations add Add<EntityName>`
7. Register repository in `Program.cs`

### Creating a Database Migration
```bash
# Create migration
dotnet ef migrations add AddFeatureX

# Update database (automatic on startup)
dotnet run
```

### Adding Logging
Inject `ILogger<Program>`:
```csharp
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Message");
```

### Adding Services
In `Program.cs` before `var app = app.Build()`:
```csharp
builder.Services.AddScoped<IMyService, MyService>();
```

## Authentication & JWT

### Overview
Implements JWT authentication with bcrypt password hashing using Entity Framework code-first approach with SQLite. Users can register and receive JWT tokens for authentication.

### Architecture
- User data persists in SQLite database
- `IUserRepository` abstracts database access
- `UserRepository` implements data operations using Entity Framework
- Password hashing via BCrypt.Net-Next (never plain text)

### Endpoints
- **POST /auth/register**: Register a new user
  - Request: `{ "name": string, "email": string, "password": string }`
  - Response: `{ "message": string, "userId": guid }`
  - Validates: Email uniqueness (enforced by database index)
  
- **POST /auth/authenticate**: Authenticate and receive JWT token
  - Request: `{ "email": string, "password": string }`
  - Response: `{ "token": string }` (JWT valid for 24 hours)
  - Status: 401 Unauthorized if credentials invalid

### JWT Configuration
JWT settings in `appsettings.json`:
```json
"Jwt": {
  "Key": "your-secret-key-min-32-characters-long",
  "Issuer": "BolaoApi",
  "Audience": "BolaoApiUsers"
}
```

**Important**: Change the `Key` to a strong secret in production.

### User Model
Located in `Domain/Entities/User.cs`:
- **Id**: Unique identifier (GUID)
- **Name**: User's full name (string)
- **Email**: User email (string, unique indexed)
- **PasswordHash**: Bcrypt-hashed password
- **CreatedAt**: Timestamp (UTC)

### Dependency Injection
In `Program.cs`:
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddScoped<IUserRepository, UserRepository>();
```

### Password Handling
- Passwords are hashed using BCrypt.Net-Next during registration
- During authentication, `BCrypt.Verify()` validates plain text against hash
- Never store or log plain text passwords

## Recommended MCP Servers

Configure these MCP servers in your Copilot setup for enhanced development:

- **sqlite**: For managing SQLite databases if you add local data persistence
- **postgres**: For PostgreSQL database integration and schema management
- **github**: For repository operations, issue tracking, and CI/CD workflows
- **git-operations**: For advanced git workflows and repository analysis
