# 🚀 ManagementApp - Ứng dụng Quản lý Người dùng

Ứng dụng quản lý người dùng được xây dựng bằng **C# .NET 8.0** với kiến trúc **Clean Architecture**, kết nối với **PostgreSQL**, có độ bảo mật cao, có thể tái sử dụng và tích hợp vào bất kỳ ứng dụng nào.

---

## 📋 Mục lục

- [Tổng quan](#-tổng-quan)
- [Yêu cầu hệ thống](#-yêu-cầu-hệ-thống)
- [Cài đặt và Setup](#-cài-đặt-và-setup)
- [Cấu hình Database](#-cấu-hình-database)
- [Kiến trúc](#-kiến-trúc)
- [Cấu trúc thư mục](#-cấu-trúc-thư-mục)
- [Chạy ứng dụng](#-chạy-ứng-dụng)
- [Packages đã sử dụng](#-packages-đã-sử-dụng)
- [Roadmap](#-roadmap)
- [Tài liệu tham khảo](#-tài-liệu-tham-khảo)

---

## 🎯 Tổng quan

Đây là một dự án học tập và phát triển từng bước để xây dựng một hệ thống quản lý người dùng hoàn chỉnh, từ cơ bản đến nâng cao, bao gồm:

- ✅ **Clean Architecture** - Kiến trúc rõ ràng, dễ bảo trì
- ✅ **PostgreSQL Integration** - Kết nối database với schema riêng
- ✅ **Dependency Injection** - Quản lý dependencies theo best practices
- ✅ **Entity Framework Core** - ORM cho database operations
- 🔄 **SOLID Principles** - Áp dụng các nguyên tắc thiết kế
- 🔄 **CQRS Pattern** - Tách biệt Commands và Queries
- 🔄 **Authentication & Authorization** - Xác thực và phân quyền
- 🔄 **Testing** - Unit tests và Integration tests

---

## 💻 Yêu cầu hệ thống

- **.NET SDK 8.0** hoặc cao hơn
- **PostgreSQL 12+** hoặc cao hơn
- **Git** (để clone repository)
- **IDE**: Visual Studio 2022, Rider, hoặc VS Code với C# extension

### Kiểm tra .NET SDK

```bash
dotnet --version
# Kết quả mong đợi: 8.0.x hoặc cao hơn
```

---

## 🛠️ Cài đặt và Setup

### Bước 1: Tạo Project mới

```bash
# Tạo thư mục dự án
mkdir ManagementApp
cd ManagementApp

# Tạo console application
dotnet new console -n ManagementApp
```

### Bước 2: Cấu hình Project File

Cập nhật `ManagementApp.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net8.0</TargetFramework>
        <Nullable>enable</Nullable>
        <ImplicitUsings>enable</ImplicitUsings>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
        <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0">
            <PrivateAssets>all</PrivateAssets>
            <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
        </PackageReference>
        <PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="8.0.0" />
        <PackageReference Include="Microsoft.Extensions.Hosting" Version="8.0.0" />
        <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
    </ItemGroup>

</Project>
```

### Bước 3: Restore Packages

```bash
dotnet restore
```

### Bước 4: Tạo cấu trúc Clean Architecture

Tạo các thư mục theo cấu trúc sau:

```bash
# Domain Layer
mkdir -p src/Domain/Entities
mkdir -p src/Domain/ValueObjects
mkdir -p src/Domain/Interfaces
mkdir -p src/Domain/Exceptions
mkdir -p src/Domain/Common

# Application Layer
mkdir -p src/Application/UseCases/Commands
mkdir -p src/Application/UseCases/Queries
mkdir -p src/Application/DTOs
mkdir -p src/Application/Interfaces
mkdir -p src/Application/Mappings
mkdir -p src/Application/Validators
mkdir -p src/Application/Common

# Infrastructure Layer
mkdir -p src/Infrastructure/Data/Repositories
mkdir -p src/Infrastructure/Data/Configurations
mkdir -p src/Infrastructure/Data/Migrations
mkdir -p src/Infrastructure/Services/Email
mkdir -p src/Infrastructure/Services/Storage
mkdir -p src/Infrastructure/Services/External
mkdir -p src/Infrastructure/Persistence
mkdir -p src/Infrastructure/Common

# Presentation Layer
mkdir -p src/Presentation/Console
mkdir -p src/Presentation/API
mkdir -p src/Presentation/Common
```

---

## 🗄️ Cấu hình Database

### Bước 1: Tạo Database và Schema trong PostgreSQL

```sql
-- Kết nối PostgreSQL
psql -U postgres

-- Tạo database
CREATE DATABASE "ManagementApp";

-- Kết nối vào database
\c ManagementApp

-- Tạo schema
CREATE SCHEMA IF NOT EXISTS "ManagementApp";

-- Cấp quyền cho user (thay YOUR_USER bằng username của bạn)
GRANT ALL PRIVILEGES ON SCHEMA "ManagementApp" TO YOUR_USER;
```

### Bước 2: Cấu hình Connection String

Tạo file `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=aubot-fms;Database=ManagementApp;Username=YOUR_USER;Password=YOUR_PASSWORD;SearchPath=ManagementApp"
  }
}
```

**Lưu ý**: Thay thế:

- `aubot-fms` → Tên server PostgreSQL của bạn
- `YOUR_USER` → Username PostgreSQL của bạn
- `YOUR_PASSWORD` → Password PostgreSQL của bạn

### Bước 3: Tạo ApplicationDbContext

Tạo file `src/Infrastructure/Data/ApplicationDbContext.cs`:

```csharp
using Microsoft.EntityFrameworkCore;

namespace ManagementApp.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure default schema
        modelBuilder.HasDefaultSchema("ManagementApp");

        // Apply entity configurations here
        // modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
```

### Bước 4: Tạo DependencyInjection Extension

Tạo file `src/Infrastructure/DependencyInjection.cs`:

```csharp
using ManagementApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ManagementApp.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Đăng ký các services của Infrastructure layer
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure PostgreSQL Database
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "ManagementApp");
            }));

        // TODO: Đăng ký repositories ở đây
        // services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
```

---

## 🏗️ Kiến trúc

Dự án sử dụng **Clean Architecture** (Onion Architecture) với 4 layers:

```
┌─────────────────────────────────────┐
│      Presentation Layer             │  ← User Interface (Console, API)
├─────────────────────────────────────┤
│      Application Layer               │  ← Use Cases, Business Rules
├─────────────────────────────────────┤
│      Infrastructure Layer            │  ← Database, External Services
├─────────────────────────────────────┤
│      Domain Layer                    │  ← Core Business Logic
└─────────────────────────────────────┘
```

### Dependency Flow

```
Presentation → Application → Domain
     ↓              ↓
Infrastructure ────┘
```

**Quy tắc**:

- ✅ Dependencies chỉ hướng vào trong
- ✅ Domain không phụ thuộc vào bất kỳ layer nào
- ✅ Application chỉ phụ thuộc vào Domain
- ✅ Infrastructure và Presentation phụ thuộc vào Application và Domain

Xem chi tiết tại [ARCHITECTURE.md](./ARCHITECTURE.md)

---

## 📁 Cấu trúc thư mục

```
ManagementApp/
├── src/
│   ├── Domain/                    # Core Business Logic (Innermost)
│   │   ├── Entities/             # Domain entities
│   │   ├── ValueObjects/         # Value objects
│   │   ├── Interfaces/           # Repository & Service interfaces
│   │   ├── Exceptions/           # Domain exceptions
│   │   └── Common/               # Base classes, Enums
│   │
│   ├── Application/               # Use Cases & Business Rules
│   │   ├── UseCases/
│   │   │   ├── Commands/         # CQRS Commands (Create, Update, Delete)
│   │   │   └── Queries/          # CQRS Queries (Read operations)
│   │   ├── DTOs/                 # Data Transfer Objects
│   │   ├── Interfaces/           # Application service interfaces
│   │   ├── Mappings/             # AutoMapper profiles
│   │   ├── Validators/           # FluentValidation validators
│   │   └── Common/                # Application utilities
│   │
│   ├── Infrastructure/            # External Concerns
│   │   ├── Data/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   ├── Repositories/     # Repository implementations
│   │   │   ├── Configurations/   # EF Core entity configurations
│   │   │   └── Migrations/       # Database migrations
│   │   ├── Services/
│   │   │   ├── Email/            # Email service implementations
│   │   │   ├── Storage/          # File storage implementations
│   │   │   └── External/         # Third-party service integrations
│   │   ├── DependencyInjection.cs
│   │   └── Common/               # Infrastructure utilities
│   │
│   └── Presentation/              # User Interface
│       ├── Console/              # Console application (hiện tại)
│       ├── API/                  # Web API (tương lai)
│       └── Common/               # Presentation utilities
│
├── Program.cs                     # Entry Point
├── appsettings.json              # Configuration
├── ManagementApp.csproj          # Project file
├── README.md                     # File này
├── ARCHITECTURE.md               # Tài liệu kiến trúc
├── ROADMAP.md                    # Lộ trình phát triển
└── SECURITY.md                   # Tài liệu bảo mật
```

---

## 🚀 Chạy ứng dụng

### Bước 1: Cấu hình Connection String

Đảm bảo `appsettings.json` có connection string đúng:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=aubot-fms;Database=ManagementApp;Username=YOUR_USER;Password=YOUR_PASSWORD;SearchPath=ManagementApp"
  }
}
```

### Bước 2: Tạo Program.cs

Tạo file `Program.cs`:

```csharp
using ManagementApp.Infrastructure;
using ManagementApp.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Đăng ký Infrastructure services (Database, Repositories, External Services)
        services.AddInfrastructure(context.Configuration);

        // TODO: Đăng ký Application services ở đây
        // services.AddApplication();

        // Debug: Hiển thị thông tin cấu hình
        var environment = context.HostingEnvironment.EnvironmentName;
        Console.WriteLine($"🔧 Environment: {environment}");
        Console.WriteLine($"📁 Đang đọc từ: appsettings.json và appsettings.{environment}.json");

        var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(connectionString))
        {
            // Ẩn password trong log
            var safeConnectionString = connectionString.Contains("Password=")
                ? connectionString.Substring(0, connectionString.IndexOf("Password=")) + "Password=***"
                : connectionString;
            Console.WriteLine($"🔗 Connection String: {safeConnectionString}");
        }
    })
    .Build();

// Test database connection
using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        var canConnect = await dbContext.Database.CanConnectAsync();
        if (canConnect)
        {
            Console.WriteLine("✅ Kết nối database PostgreSQL thành công!");
        }
        else
        {
            Console.WriteLine("❌ Không thể kết nối database.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Lỗi kết nối database: {ex.Message}");
    }
}

await host.RunAsync();
```

### Bước 3: Chạy ứng dụng

```bash
dotnet run
```

**Kết quả mong đợi**:

```
🔧 Environment: Production
📁 Đang đọc từ: appsettings.json và appsettings.Production.json
🔗 Connection String: Host=aubot-fms;Database=ManagementApp;Username=YOUR_USER;Password=***
✅ Kết nối database PostgreSQL thành công!
```

---

## 📦 Packages đã sử dụng

| Package                                           | Version | Mục đích                        |
| ------------------------------------------------- | ------- | ------------------------------- |
| `Microsoft.EntityFrameworkCore`                   | 8.0.0   | Entity Framework Core ORM       |
| `Microsoft.EntityFrameworkCore.Design`            | 8.0.0   | EF Core design-time tools       |
| `Microsoft.Extensions.Configuration.Abstractions` | 8.0.0   | Configuration abstractions      |
| `Microsoft.Extensions.Hosting`                    | 8.0.0   | Hosting và Dependency Injection |
| `Npgsql.EntityFrameworkCore.PostgreSQL`           | 8.0.0   | PostgreSQL provider cho EF Core |

---

## 🗺️ Roadmap

Xem chi tiết lộ trình phát triển tại [ROADMAP.md](./ROADMAP.md)

### Trạng thái hiện tại

- ✅ **Phase 1.1**: Console Application với Hello World
- ✅ **Phase 1.2**: Tổ chức thư mục theo Clean Architecture
- ✅ **Phase 2.1**: Database Setup với PostgreSQL
- ⏭️ **Phase 1.3**: Models và Entities
- ⏭️ **Phase 1.4**: SOLID Principles
- ⏭️ **Phase 2.2**: Migrations và Schema
- ⏭️ **Phase 2.3**: CRUD Operations

---

## 📚 Tài liệu tham khảo

### Kiến trúc

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [.NET Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture)

### Entity Framework Core

- [EF Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [PostgreSQL Provider](https://www.npgsql.org/efcore/)

### .NET

- [.NET Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [Dependency Injection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)

---

## 🔒 Bảo mật

Xem chi tiết về bảo mật tại [SECURITY.md](./SECURITY.md)

**Lưu ý quan trọng**:

- ⚠️ **KHÔNG** commit file `appsettings.json` có chứa password thật vào Git
- ✅ Sử dụng `appsettings.Development.json.example` làm template
- ✅ Sử dụng User Secrets cho development: `dotnet user-secrets set "ConnectionStrings:DefaultConnection" "..."`

---

## 🤝 Đóng góp

Dự án này là một dự án học tập. Mọi đóng góp đều được chào đón!

---

## 📄 License

MIT License - Xem file LICENSE để biết thêm chi tiết.

---

## 👤 Tác giả

Được tạo như một phần của lộ trình học tập Clean Architecture và .NET.

---

**Happy Coding! 🚀**
