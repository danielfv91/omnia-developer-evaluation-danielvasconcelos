# Functional Tests - Developer Evaluation Project

This guide explains how to run and understand integration tests for the project.

## Prerequisites
- Docker Desktop running
- Image `postgres:latest` available
- .NET 8 SDK installed (`dotnet --version`)
- Project builds successfully (`dotnet build`)

## Run the tests
```bash
dotnet test tests/Ambev.DeveloperEvaluation.Integration
```

## How it works
- Each test spins up a **PostgreSQL container** with `Testcontainers`
- `CustomWebApplicationFactory` replaces `DbContext` connection
- App runs in memory (`TestServer`) with Serilog, AutoMapper, and middleware
- Tests use a real `HttpClient` to simulate users

## Structure
- `SalesEndpointTests.cs`: covers create, get, update, delete
- `PostgreSqlContainerFactory.cs`: container factory
- `CustomWebApplicationFactory.cs`: test server setup

## Troubleshooting
| Error               | Cause         | Solution             |
|---------------------|---------------|----------------------|
| Container not ready | Docker is off | Start Docker Desktop |
| pg_isready fails    | Port in use   | Free or change port  |

## Covered Endpoints
- `GET /api/sales`
- `GET /api/sales/{id}`
- `POST /api/sales`
- `PUT /api/sales/{id}`
- `DELETE /api/sales/{id}`

## Notes
- No local DB required — only Docker