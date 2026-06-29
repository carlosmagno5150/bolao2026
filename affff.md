# Pacotes NuGet — Referência

Instale apenas o que for necessário para o contexto do usuário. Não adicione pacotes sem justificativa.

## ORM / Banco de Dados

| Pacote | Comando | Uso |
|---|---|---|
| EF Core (base) | `dotnet add package Microsoft.EntityFrameworkCore` | ORM principal |
| EF Core — SQL Server | `dotnet add package Microsoft.EntityFrameworkCore.SqlServer` | Banco SQL Server |
| EF Core — PostgreSQL | `dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL` | Banco PostgreSQL |
| EF Core — SQLite | `dotnet add package Microsoft.EntityFrameworkCore.Sqlite` | Banco SQLite (dev/testes) |
| EF Core — Tools | `dotnet add package Microsoft.EntityFrameworkCore.Tools` | Migrations via CLI |
| EF Core — Design | `dotnet add package Microsoft.EntityFrameworkCore.Design` | Geração de migrations |
| Dapper | `dotnet add package Dapper` | Micro-ORM para queries SQL cruas |

## CQRS / Mediator

| Pacote | Comando | Uso |
|---|---|---|
| MediatR | `dotnet add package MediatR` | Implementação do padrão Mediator/CQRS |
| MediatR.Extensions.DI | `dotnet add package MediatR.Extensions.Microsoft.DependencyInjection` | Integração com DI (versões < 12) |

> A partir do MediatR 12+, o registro de DI já está incluso no pacote principal.

## Validação

| Pacote | Comando | Uso |
|---|---|---|
| FluentValidation | `dotnet add package FluentValidation` | Validação fluente de modelos |
| FluentValidation.AspNetCore | `dotnet add package FluentValidation.AspNetCore` | Integração com ASP.NET Core pipeline |

## Mapeamento

| Pacote | Comando | Uso |
|---|---|---|
| AutoMapper | `dotnet add package AutoMapper` | Mapeamento automático entre objetos |
| AutoMapper.Extensions.DI | `dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection` | Integração com DI |

> A partir do AutoMapper 13+, o registro de DI já está incluso no pacote principal.

## Logging

| Pacote | Comando | Uso |
|---|---|---|
| Serilog.AspNetCore | `dotnet add package Serilog.AspNetCore` | Serilog integrado ao ASP.NET Core |
| Serilog.Sinks.Console | `dotnet add package Serilog.Sinks.Console` | Log no console |
| Serilog.Sinks.File | `dotnet add package Serilog.Sinks.File` | Log em arquivo |
| Serilog.Sinks.Seq | `dotnet add package Serilog.Sinks.Seq` | Log no Seq (visualizador) |

## HTTP Clients

| Pacote | Comando | Uso |
|---|---|---|
| Refit | `dotnet add package Refit` | HTTP client declarativo via interfaces |
| Refit.HttpClientFactory | `dotnet add package Refit.HttpClientFactory` | Integração com IHttpClientFactory |
| Polly | `dotnet add package Microsoft.Extensions.Http.Polly` | Resiliência (retry, circuit breaker) |

## Autenticação / Autorização

| Pacote | Comando | Uso |
|---|---|---|
| JWT Bearer | `dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer` | Autenticação via JWT |

## Testes

| Pacote | Comando | Uso |
|---|---|---|
| xUnit | `dotnet add package xunit` | Framework de testes (já incluso no template xunit) |
| FluentAssertions | `dotnet add package FluentAssertions` | Asserções legíveis |
| Moq | `dotnet add package Moq` | Mocking |
| NSubstitute | `dotnet add package NSubstitute` | Alternativa ao Moq |
| Bogus | `dotnet add package Bogus` | Geração de dados falsos para testes |
| EF Core InMemory | `dotnet add package Microsoft.EntityFrameworkCore.InMemory` | DB em memória para testes |

## Utilitários

| Pacote | Comando | Uso |
|---|---|---|
| Newtonsoft.Json | `dotnet add package Newtonsoft.Json` | JSON (alternativa ao System.Text.Json) |
| Swashbuckle | `dotnet add package Swashbuckle.AspNetCore` | Swagger/OpenAPI (já incluso no template webapi) |
| HealthChecks | `dotnet add package Microsoft.Extensions.Diagnostics.HealthChecks` | Health checks |

---

## Exemplo: instalar pacotes para Clean Architecture com EF Core + MediatR + Serilog

```bash
# Application
dotnet add ./src/MinhaApp.Application/MinhaApp.Application.csproj package MediatR
dotnet add ./src/MinhaApp.Application/MinhaApp.Application.csproj package FluentValidation
dotnet add ./src/MinhaApp.Application/MinhaApp.Application.csproj package AutoMapper

# Infrastructure
dotnet add ./src/MinhaApp.Infrastructure/MinhaApp.Infrastructure.csproj package Microsoft.EntityFrameworkCore
dotnet add ./src/MinhaApp.Infrastructure/MinhaApp.Infrastructure.csproj package Microsoft.EntityFrameworkCore.SqlServer
dotnet add ./src/MinhaApp.Infrastructure/MinhaApp.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Tools

# Api
dotnet add ./src/MinhaApp.Api/MinhaApp.Api.csproj package Serilog.AspNetCore
dotnet add ./src/MinhaApp.Api/MinhaApp.Api.csproj package Serilog.Sinks.Console
dotnet add ./src/MinhaApp.Api/MinhaApp.Api.csproj package FluentValidation.AspNetCore
```
