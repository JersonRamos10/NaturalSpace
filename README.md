# NaturalSpaceApi

Backend API for NaturalSpace - a workspace and channel messaging system.

## Tech Stack

- **.NET 10** - ASP.NET Core Web API
- **Entity Framework Core 10** - ORM
- **SQL Server** - Database
- **Docker Compose** - Container orchestration
- **Clean Architecture** - Project structure

## Project Structure

```
NaturalSpaceApi/
├── NaturalSpaceApi/                    # API Layer
├── NaturalSpaceApi.Domain/             # Domain Entities
├── NaturalSpaceApi.Application/        # Application Services, DTOs, Interfaces
└── NaturalSpaceApi.Infraestructure/    # EF Core, Configurations, Migrations
```

## Quick Start

### Prerequisites

- .NET 10 SDK
- Docker Desktop (for containerized database)
- SQL Server (local or container)

### Option 1: Run with Docker (Recommended)

```bash
# Start SQL Server container
docker compose up -d sqlserver

# Run API
dotnet run --project NaturalSpaceApi

# Or run everything with docker-compose
docker compose up -d
```

### Option 2: Run with Local SQL Server

1. Ensure SQL Server is running on `localhost,1433`
2. Update `appsettings.Development.json` with your credentials
3. Run the API:

```bash
dotnet run --project NaturalSpaceApi
```

## Configuration

### Connection Strings

#### Docker Compose (between containers)
```json
Server=sqlserver;Database=NspacesDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;
```

#### Local Development
```json
Server=localhost,1433;Database=NspacesDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;
```

#### SQL Server Express Local
```json
Server=.\\SQLEXPRESS;Database=NspacesDb;Integrated Security=True;TrustServerCertificate=True;
```

### Environment Variables

| Variable | Description |
|----------|-------------|
| `ASPNETCORE_ENVIRONMENT` | Development / Production |
| `ASPNETCORE_HTTP_PORTS` | HTTP port (default: 8080) |
| `ASPNETCORE_AUTO_MIGRATE` | Auto-apply migrations on startup |

### Configuration Files

⚠️ **Important**: Do not commit files containing real credentials.

The following files are ignored by Git:
- `appsettings.json`
- `appsettings.Development.json`
- `appsettings.Production.json`

Create your own configuration or use environment variables.

## Database

### Run Migrations

```bash
# Create migration
dotnet ef migrations add MigrationName \
  --project NaturalSpaceApi.Infraestructure \
  --startup-project NaturalSpaceApi

# Apply migrations
dotnet ef database update \
  --project NaturalSpaceApi.Infraestructure \
  --startup-project NaturalSpaceApi
```

### SQL Server Management Studio

| Field | Value |
|-------|-------|
| Server | `localhost,1433` |
| Auth | SQL Server Authentication |
| Login | `sa` |
| Password | (your configured password) |

## Development

### Build

```bash
dotnet build NaturalSpaceApi.slnx
```

### Run Tests

```bash
dotnet test
```

### Useful Commands

```bash
# Restore packages
dotnet restore NaturalSpaceApi.slnx

# Watch mode
dotnet watch --project NaturalSpaceApi

# Clean and rebuild
dotnet clean NaturalSpaceApi.slnx
dotnet build NaturalSpaceApi.slnx
```

## Environment Setup

### Windows

1. Install .NET 10 SDK
2. Install Docker Desktop
3. Clone repository
4. Copy `appsettings.json` to `appsettings.Development.json` and configure credentials
5. Run `docker compose up -d sqlserver`
6. Run `dotnet ef database update` to apply migrations
7. Run `dotnet run --project NaturalSpaceApi`

### Linux/Mac

1. Install .NET 10 SDK
2. Install Docker
3. Clone repository
4. Configure `appsettings.Development.json`
5. Run `docker compose up -d sqlserver`
6. Apply migrations and run

## License

MIT
