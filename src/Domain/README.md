# Domain Layer

## 📋 Mô tả

Domain Layer là lớp trong cùng của Clean Architecture, chứa **business logic** và **business rules** của ứng dụng. Layer này **KHÔNG phụ thuộc** vào bất kỳ layer nào khác.

## 📁 Cấu trúc thư mục

```
Domain/
├── Entities/          # Domain entities (business objects)
├── ValueObjects/      # Value objects (immutable objects)
├── Interfaces/        # Repository interfaces và domain service interfaces
├── Exceptions/        # Custom domain exceptions
└── Common/           # Base classes, enums, constants
```

## 🎯 Nguyên tắc

- ✅ **KHÔNG** có dependency vào Infrastructure, Application, hay Presentation
- ✅ Chỉ chứa business logic thuần túy
- ✅ Entities và ValueObjects là POCO (Plain Old CLR Objects)
- ✅ Interfaces định nghĩa contracts, không phải implementations

## 📝 Ví dụ

### Entity

```csharp
public class User
{
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public Email Email { get; private set; } // ValueObject

    // Business logic methods
    public void ChangePassword(string newPassword) { ... }
}
```

### Repository Interface

```csharp
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(Email email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
}
```
