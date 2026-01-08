# 🏗️ Clean Architecture - Cấu trúc Dự án

## 📋 Tổng quan

Dự án được tổ chức theo mô hình **Clean Architecture** (Onion Architecture), đảm bảo:
- **Separation of Concerns**: Mỗi layer có trách nhiệm riêng biệt
- **Dependency Rule**: Dependencies chỉ hướng vào trong (từ ngoài vào trong)
- **Testability**: Dễ dàng test từng layer độc lập
- **Maintainability**: Code dễ bảo trì và mở rộng

---

## 🎯 Cấu trúc Layers

```
ManagementApp/
├── src/
│   ├── Domain/              # Core Business Logic (Innermost)
│   ├── Application/         # Use Cases & Business Rules
│   ├── Infrastructure/      # External Concerns
│   └── Presentation/        # User Interface (Outermost)
├── Program.cs               # Entry Point
└── appsettings.json         # Configuration
```

---

## 📁 Chi tiết từng Layer

### 1. 🎯 Domain Layer (Core)
**Vị trí**: `src/Domain/`  
**Mục đích**: Chứa business logic cốt lõi, không phụ thuộc vào bất kỳ layer nào khác.

**Cấu trúc**:
```
Domain/
├── Entities/           # Domain entities (User, Product, Order, ...)
├── ValueObjects/       # Value objects (Email, Address, Money, ...)
├── Interfaces/         # Repository interfaces, Service interfaces
├── Exceptions/         # Domain-specific exceptions
└── Common/             # Base classes, Enums, Constants
```

**Nguyên tắc**:
- ✅ Không phụ thuộc vào bất kỳ layer nào
- ✅ Chỉ chứa business logic thuần túy
- ✅ Không có reference đến EF Core, Database, hoặc bất kỳ framework nào

**Ví dụ**:
```csharp
// Domain/Entities/User.cs
public class User
{
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public Email Email { get; private set; } // ValueObject
    
    // Business logic methods
    public void ChangeEmail(Email newEmail) { ... }
}
```

---

### 2. 📝 Application Layer
**Vị trí**: `src/Application/`  
**Mục đích**: Chứa use cases, business rules, và orchestration logic.

**Cấu trúc**:
```
Application/
├── UseCases/
│   ├── Commands/       # CQRS Commands (Create, Update, Delete)
│   └── Queries/        # CQRS Queries (Read operations)
├── DTOs/               # Data Transfer Objects
├── Interfaces/        # Application service interfaces
├── Mappings/           # AutoMapper profiles
├── Validators/         # FluentValidation validators
└── Common/             # Application-specific utilities
```

**Nguyên tắc**:
- ✅ Phụ thuộc vào Domain layer
- ✅ Không phụ thuộc vào Infrastructure hoặc Presentation
- ✅ Chứa use case logic và business workflows

**Ví dụ**:
```csharp
// Application/UseCases/Commands/CreateUserCommand.cs
public class CreateUserCommand
{
    public string Username { get; set; }
    public string Email { get; set; }
}

// Application/UseCases/Commands/CreateUserCommandHandler.cs
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    
    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Business logic here
    }
}
```

---

### 3. 🔧 Infrastructure Layer
**Vị trí**: `src/Infrastructure/`  
**Mục đích**: Implement các interfaces từ Domain và Application, xử lý external concerns.

**Cấu trúc**:
```
Infrastructure/
├── Data/
│   ├── ApplicationDbContext.cs    # EF Core DbContext
│   ├── Repositories/              # Repository implementations
│   ├── Configurations/            # EF Core entity configurations
│   └── Migrations/                # Database migrations
├── Services/
│   ├── Email/                     # Email service implementations
│   ├── Storage/                   # File storage implementations
│   └── External/                  # Third-party service integrations
├── Persistence/                   # Database-specific code
└── Common/                        # Infrastructure utilities
```

**Nguyên tắc**:
- ✅ Phụ thuộc vào Domain và Application layers
- ✅ Implement các interfaces được định nghĩa ở Domain/Application
- ✅ Chứa tất cả code liên quan đến database, external APIs, file system

**Ví dụ**:
```csharp
// Infrastructure/Data/Repositories/UserRepository.cs
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    
    public async Task<User> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }
}
```

---

### 4. 🖥️ Presentation Layer
**Vị trí**: `src/Presentation/`  
**Mục đích**: Giao diện người dùng, entry point của ứng dụng.

**Cấu trúc**:
```
Presentation/
├── Console/           # Console application (hiện tại)
├── API/               # Web API (tương lai)
└── Common/            # Presentation utilities
```

**Nguyên tắc**:
- ✅ Phụ thuộc vào Application layer
- ✅ Không chứa business logic
- ✅ Chỉ xử lý input/output, validation, và routing

**Ví dụ**:
```csharp
// Presentation/Console/Program.cs (Entry Point)
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Register services
        services.AddApplication();
        services.AddInfrastructure(context.Configuration);
    })
    .Build();
```

---

## 🔄 Dependency Flow

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

---

## 📦 Dependency Injection Setup

### Trong Program.cs:
```csharp
services.AddApplication();        // Application layer services
services.AddInfrastructure(config); // Infrastructure layer services
services.AddPresentation();        // Presentation layer services
```

### Extension Methods:
```csharp
// Application/Extensions/ServiceCollectionExtensions.cs
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register Application services
        return services;
    }
}

// Infrastructure/Extensions/ServiceCollectionExtensions.cs
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Register Infrastructure services
        return services;
    }
}
```

---

## 🧪 Testing Strategy

```
Tests/
├── Domain.Tests/          # Unit tests cho Domain
├── Application.Tests/      # Unit tests cho Application
├── Infrastructure.Tests/   # Integration tests cho Infrastructure
└── Presentation.Tests/    # Integration tests cho Presentation
```

---

## 📚 Best Practices

1. **Entities**: Đặt trong `Domain/Entities/`
2. **DTOs**: Đặt trong `Application/DTOs/`
3. **Repositories**: Interface trong `Domain/Interfaces/`, Implementation trong `Infrastructure/Data/Repositories/`
4. **Use Cases**: Đặt trong `Application/UseCases/`
5. **Configuration**: Đặt trong `Infrastructure/Data/Configurations/`

---

## 🚀 Next Steps

1. ✅ Tạo cấu trúc thư mục
2. ⏭️ Tạo base classes và interfaces
3. ⏭️ Thiết lập Dependency Injection
4. ⏭️ Tạo User entity và repository
5. ⏭️ Implement use cases

---

## 📖 Tài liệu tham khảo

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [.NET Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture)

