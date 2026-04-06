# NaturalSpaceApi - Guía de Configuración y Migraciones

> Documentación generada durante la sesión de configuración del proyecto NaturalSpaceApi
> Fecha: 2026-04-05

---

## 📋 Índice

1. [Estructura del Proyecto](#estructura-del-proyecto)
2. [Configuración Docker Compose](#configuración-docker-compose)
3. [Migración PostgreSQL → SQL Server](#migración-postgresql--sql-server)
4. [Configuración EF Core](#configuración-ef-core)
5. [Problemas Comunes y Soluciones](#problemas-comunes-y-soluciones)
6. [Comandos Útiles](#comandos-útiles)
7. [Conexiones](#conexiones)

---

## 🏗️ Estructura del Proyecto

El proyecto sigue una arquitectura Clean Architecture con los siguientes proyectos:

```
NaturalSpaceApi/
├── NaturalSpaceApi/                    # API (Presentación)
│   ├── Controllers/
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Program.cs
│   └── Dockerfile
├── NaturalSpaceApi.Domain/             # Entidades del dominio
│   └── Entities/
│       ├── User.cs
│       ├── WorkSpace.cs
│       ├── Channel.cs
│       ├── ChannelMember.cs
│       ├── UserWorkSpace.cs
│       ├── Message.cs
│       └── File.cs
├── NaturalSpaceApi.Application/        # Casos de uso (vacío actualmente)
└── NaturalSpaceApi.Infraestructure/    # Infraestructura de datos
    ├── Data/Context/NaturalSpaceContext.cs
    └── Configurations/                 # Configuraciones Fluent API
        ├── UserConfiguration.cs
        ├── WorkSpaceConfiguration.cs
        ├── ChannelConfiguration.cs
        ├── ChannelMemberConfiguration.cs
        ├── UserWorkSpaceConfiguration.cs
        ├── MessageConfiguration.cs
        └── FileConfiguration.cs
```

### Entidades Principales

| Entidad | Descripción | Relaciones |
|---------|-------------|------------|
| **User** | Usuarios del sistema | 1:N con WorkSpaces, Messages, Files |
| **WorkSpace** | Espacios de trabajo | N:M con Users, 1:N con Channels |
| **Channel** | Canales de comunicación | N:M con Users, 1:N con Messages |
| **ChannelMember** | Miembros de canales | Relación N:M entre Users y Channels |
| **UserWorkSpace** | Usuarios en workspaces | Relación N:M entre Users y WorkSpaces |
| **Message** | Mensajes | N:1 con User, Channel; 1:N con Files |
| **File** | Archivos adjuntos | N:1 con User, Message |

---

## 🐳 Configuración Docker Compose

### Configuración Actual (SQL Server)

```yaml
services:
  # Servicio de la API
  naturalspaceapi:
    image: ${DOCKER_REGISTRY-}naturalspaceapi
    container_name: naturalspace-api
    build:
      context: .
      dockerfile: NaturalSpaceApi/Dockerfile
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_HTTP_PORTS=8080
      - ASPNETCORE_HTTPS_PORTS=8081
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=NspacesDb;User Id=sa;Password=Sudo123!;TrustServerCertificate=True;
      - ASPNETCORE_AUTO_MIGRATE=true
    ports:
      - "8080:8080"
      - "8081:8081"
    depends_on:
      sqlserver:
        condition: service_healthy
    networks:
      - naturalspace-network
    volumes:
      - ${APPDATA}/Microsoft/UserSecrets:/home/app/.microsoft/usersecrets:ro
      - ${APPDATA}/Microsoft/UserSecrets:/root/.microsoft/usersecrets:ro
      - ${APPDATA}/ASP.NET/Https:/home/app/.aspnet/https:ro
      - ${APPDATA}/ASP.NET/Https:/root/.aspnet/https:ro
    restart: unless-stopped

  # Servicio de SQL Server
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: naturalspace-sqlserver
    environment:
      - ACCEPT_EULA=Y
      - MSSQL_SA_PASSWORD=Sudo123!
      - MSSQL_PID=Developer
    ports:
      - "1433:1433"
    volumes:
      - sqlserver_data:/var/opt/mssql
    networks:
      - naturalspace-network
    restart: unless-stopped
    healthcheck:
      test: ["CMD-SHELL", "/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'Sudo123!' -Q 'SELECT 1' || exit 1"]
      interval: 10s
      timeout: 5s
      retries: 5
      start_period: 30s

volumes:
  sqlserver_data:
    driver: local

networks:
  naturalspace-network:
    driver: bridge
```

### Configuración Anterior (PostgreSQL) - Referencia

```yaml
services:
  postgres:
    image: postgres:16-alpine
    container_name: naturalspace-postgres
    environment:
      - POSTGRES_DB=NspacesDb
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=sudo123!
      - PGDATA=/var/lib/postgresql/data/pgdata
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    networks:
      - naturalspace-network
    restart: unless-stopped
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres -d NspacesDb"]
      interval: 10s
      timeout: 5s
      retries: 5
      start_period: 10s
```

---

## 🔄 Migración PostgreSQL → SQL Server

### Cambios Realizados

#### 1. Docker Compose
- Servicio cambiado: `postgres` → `sqlserver`
- Imagen: `postgres:16-alpine` → `mcr.microsoft.com/mssql/server:2022-latest`
- Variables de entorno:
  - PostgreSQL: `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD`
  - SQL Server: `ACCEPT_EULA`, `MSSQL_SA_PASSWORD`, `MSSQL_PID`
- Puerto: `5432` → `1433`
- Healthcheck adaptado a SQL Server

#### 2. Cadenas de Conexión

**appsettings.Development.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=NspacesDb;User Id=sa;Password=Sudo123!;TrustServerCertificate=True;"
  }
}
```

**Docker Compose (para contenedores):**
```
Server=sqlserver;Database=NspacesDb;User Id=sa;Password=Sudo123!;TrustServerCertificate=True;
```

#### 3. Paquetes NuGet

**Proyecto API (NaturalSpaceApi.csproj):**
```xml
<!-- Antes -->
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.1" />

<!-- Después -->
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.5" />
```

**Proyecto Infrastructure:**
```xml
<!-- Reemplazar Npgsql -->
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.5" />
```

#### 4. Program.cs

```csharp
// Antes (PostgreSQL)
builder.Services.AddDbContext<NaturalSpaceContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString);
});

// Después (SQL Server)
builder.Services.AddDbContext<NaturalSpaceContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});
```

#### 5. Configuraciones Fluent API

**PostgreSQL:**
```csharp
builder.HasIndex(c => new { c.Id, c.IsDeleted })
    .HasFilter("\"IsDeleted\" = false");  // Sintaxis PostgreSQL
```

**SQL Server:**
```csharp
builder.HasIndex(c => new { c.Id, c.IsDeleted })
    .HasFilter("[IsDeleted] = 0");  // Sintaxis SQL Server
```

---

## ⚙️ Configuración EF Core

### DbContext

```csharp
public class NaturalSpaceContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<WorkSpace> WorkSpaces { get; set; }
    public DbSet<Channel> Channels { get; set; }
    public DbSet<ChannelMember> ChannelMembers { get; set; }
    public DbSet<UserWorkSpace> UserWorkSpaces { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Domain.Entities.File> Files { get; set; }

    public NaturalSpaceContext(DbContextOptions<NaturalSpaceContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new WorkSpaceConfiguration());
        modelBuilder.ApplyConfiguration(new ChannelConfiguration());
        modelBuilder.ApplyConfiguration(new ChannelMemberConfiguration());
        modelBuilder.ApplyConfiguration(new UserWorkSpaceConfiguration());
        modelBuilder.ApplyConfiguration(new MessageConfiguration());
        modelBuilder.ApplyConfiguration(new FileConfiguration());
    }
}
```

### Patrón de Configuración Fluent API

Ejemplo de configuración N:1 con navegación bidireccional:

```csharp
internal class FileConfiguration : IEntityTypeConfiguration<File>
{
    public void Configure(EntityTypeBuilder<File> builder)
    {
        builder.ToTable("Files");

        // Clave primaria
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).ValueGeneratedNever();

        // Relación N:1 con User (UploadBy)
        builder.HasOne(f => f.UploadBy)
            .WithMany(u => u.Files)  // ¡Especificar colección de navegación!
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación N:1 con Message
        builder.HasOne(f => f.Message)
            .WithMany(m => m.Attachments)  // ¡Especificar colección de navegación!
            .HasForeignKey(f => f.MessageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

---

## ⚠️ Problemas Comunes y Soluciones

### 1. Propiedades Shadow (Shadow Properties)

**Problema:**
```
The foreign key property 'File.MessageId1' was created in shadow state...
```

**Causa:** Relaciones N:1 configuradas sin especificar la colección de navegación inversa.

**Solución:**
```csharp
// ❌ Incorrecto - Crea propiedades shadow
builder.HasOne(f => f.UploadBy)
    .WithMany()  // Vacío
    .HasForeignKey(f => f.UserId);

// ✅ Correcto - Especifica la colección
builder.HasOne(f => f.UploadBy)
    .WithMany(u => u.Files)  // Colección definida
    .HasForeignKey(f => f.UserId);
```

### 2. Duplicación de Columnas

**Problema:**
```
Column name 'WorkSpaceId' in table 'Channels' is specified more than once.
```

**Causa:** Inconsistencia en nombres de propiedades (mayúsculas/minúsculas).

**Solución:** Usar nombres consistentes:
```csharp
// ❌ Inconsistente
public Guid WorkspaceId { get; set; }  // 'W' minúscula
public WorkSpace WorkSpace { get; set; }  // 'S' mayúscula

// ✅ Consistente
public Guid WorkSpaceId { get; set; }
public WorkSpace WorkSpace { get; set; }
```

### 3. Sintaxis de Filtros de Índice

**PostgreSQL:**
```csharp
.HasFilter("\"IsDeleted\" = false");  // Comillas dobles + boolean
```

**SQL Server:**
```csharp
.HasFilter("[IsDeleted] = 0");  // Corchetes + bit (0/1)
```

### 4. Connection String para Migraciones

**Desde máquina local:**
```
Server=localhost,1433;Database=NspacesDb;User Id=sa;Password=Sudo123!;TrustServerCertificate=True;
```

**Dentro de Docker (entre contenedores):**
```
Server=sqlserver;Database=NspacesDb;User Id=sa;Password=Sudo123!;TrustServerCertificate=True;
```

> **Nota:** En Docker Compose, los contenedores se comunican por nombre de servicio (`sqlserver`), no por `localhost`.

---

## 💻 Comandos Útiles

### Docker

```bash
# Levantar contenedores
docker compose up -d

# Levantar solo SQL Server
docker compose up -d sqlserver

# Ver logs
docker logs naturalspace-sqlserver

# Ver estado de contenedores
docker ps

# Detener y eliminar
docker compose down

# Detener y eliminar volúmenes (⚠️ borra datos)
docker compose down -v
```

### Entity Framework Core

```bash
# Crear migración
dotnet ef migrations add NombreMigracion \
  --project NaturalSpaceApi.Infraestructure \
  --startup-project NaturalSpaceApi

# Aplicar migraciones
dotnet ef database update \
  --project NaturalSpaceApi.Infraestructure \
  --startup-project NaturalSpaceApi

# Eliminar última migración
dotnet ef migrations remove \
  --project NaturalSpaceApi.Infraestructure \
  --startup-project NaturalSpaceApi \
  --force

# Ver migraciones pendientes
dotnet ef migrations list \
  --project NaturalSpaceApi.Infraestructure \
  --startup-project NaturalSpaceApi
```

### Compilación

```bash
# Restaurar paquetes
dotnet restore NaturalSpaceApi.slnx

# Compilar solución
dotnet build NaturalSpaceApi.slnx

# Ejecutar API
dotnet run --project NaturalSpaceApi
```

---

## 🔌 Conexiones

### SQL Server Management Studio (SSMS)

| Campo | Valor |
|-------|-------|
| **Server name** | `localhost,1433` |
| **Authentication** | SQL Server Authentication |
| **Login** | `sa` |
| **Password** | `Sudo123!` |

### Visual Studio (Server Explorer)

```
Server: localhost,1433
Database: NspacesDb
Authentication: SQL Server Authentication
User: sa
Password: Sudo123!
```

### Connection Strings

**Desarrollo local (API en máquina, SQL en Docker):**
```json
"DefaultConnection": "Server=localhost,1433;Database=NspacesDb;User Id=sa;Password=Sudo123!;TrustServerCertificate=True;"
```

**Todo en Docker Compose:**
```yaml
- ConnectionStrings__DefaultConnection=Server=sqlserver;Database=NspacesDb;User Id=sa;Password=Sudo123!;TrustServerCertificate=True;
```

**SQL Express local (sin Docker):**
```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=NspacesDb;Integrated Security=True;TrustServerCertificate=True;"
```

---

## 📚 Referencias

- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core/)
- [SQL Server Docker Image](https://hub.docker.com/_/microsoft-mssql-server)
- [PostgreSQL Docker Image](https://hub.docker.com/_/postgres)
- [Docker Compose Documentation](https://docs.docker.com/compose/)

---

## 📝 Notas

- El usuario `sa` (System Administrator) es el único usuario creado automáticamente por la imagen Docker de SQL Server
- Para producción, considerar crear usuarios específicos con permisos limitados
- Las migraciones automáticas (`ASPNETCORE_AUTO_MIGRATE=true`) son útiles solo en desarrollo
- Siempre verificar que los nombres de propiedades sean consistentes en mayúsculas/minúsculas

---

*Documento generado automáticamente durante la configuración del proyecto NaturalSpaceApi*
