# BolaoApi

BolaoApi is an ASP.NET Core 10.0 minimal API implementing a simple betting/pool (bolão) backend using DDD principles, EF Core (SQLite), JWT authentication and a repository pattern.

## Table of Contents
- Overview
- Features
- Prerequisites
- Build & Run
- Configuration
- Database & Migrations
- API Endpoints
- Project Structure
- Contributing
- License

## Overview
This project provides user registration and authentication with JWT, domain-driven structure, and lightweight minimal API endpoints defined in Program.cs. It uses SQLite for persistence and applies EF Core migrations on startup.

## Features
- Minimal API (no controllers)
- EF Core (SQLite) with code-first migrations
- JWT authentication
- Bcrypt password hashing
- Repository pattern and DI

## Prerequisites
- .NET 10.0 SDK
- Optional: dotnet-ef tool for migrations (dotnet tool install --global dotnet-ef)

## Build
From repository root:

    dotnet build

## Run
Development profile (HTTP):

    dotnet run --project BolaoApi/BolaoApi.csproj --launch-profile http

Default HTTPS profile:

    dotnet run --project BolaoApi/BolaoApi.csproj --launch-profile https

When running in Development, OpenAPI is available at:

    http://localhost:5010/openapi/v1.json

## Configuration
App configuration lives in `appsettings.json` and `appsettings.Development.json`.
Important JWT settings example:

```json
"Jwt": {
  "Key": "your-secret-key-min-32-characters-long",
  "Issuer": "BolaoApi",
  "Audience": "BolaoApiUsers"
}
```

Replace the Key with a secure secret in production.

## Database & Migrations
The project uses SQLite (default file: `bolao.db`). Migrations are applied automatically on startup via `dbContext.Database.MigrateAsync()`.

To create a migration manually:

    dotnet ef migrations add AddMyFeature --project BolaoApi --startup-project BolaoApi

To update the DB manually:

    dotnet ef database update --project BolaoApi --startup-project BolaoApi

## API Endpoints (examples)
- POST /auth/register  — register a new user
- POST /auth/authenticate — authenticate and receive a JWT token
- GET /weatherforecast — example endpoint (from template)

Use the `BolaoApi.http` file in the repository root to try endpoints locally.

## Project Structure
- Domain/: domain entities and interfaces
- Application/: DTOs and use-case logic
- Infrastructure/: EF DbContext, repositories and data access
- BolaoApi/: API project (Program.cs, configuration)

## Contributing
- Follow repository coding conventions (nullable enabled, async EF calls)
- Add DTOs to Application/Dtos, entities to Domain/Entities, repository contracts to Domain/Interfaces and implementations to Infrastructure/Repositories
- Register services in Program.cs
- Run tests/build before opening PRs

## License
Add your preferred license (e.g., MIT) in a LICENSE file.

---

If any specific documentation, diagrams or additional endpoints should be added to this README, provide the details and they will be included.
