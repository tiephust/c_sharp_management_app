# Application Layer

## 📋 Mô tả

Application Layer chứa **use cases** và **application logic**. Layer này điều phối Domain entities để thực hiện các tác vụ cụ thể của ứng dụng.

## 📁 Cấu trúc thư mục

```
Application/
├── UseCases/          # Use cases (Commands, Queries)
│   ├── Commands/     # Write operations (Create, Update, Delete)
│   └── Queries/      # Read operations (Get, List, Search)
├── DTOs/             # Data Transfer Objects
├── Interfaces/        # Application service interfaces
├── Mappings/          # AutoMapper profiles
├── Validators/        # FluentValidation validators
└── Common/           # Application-specific base classes
```

## 🎯 Nguyên tắc

- ✅ **Phụ thuộc vào** Domain Layer
- ✅ **KHÔNG phụ thuộc** vào Infrastructure hay Presentation
- ✅ Use cases là single-purpose classes (một use case = một class)
- ✅ DTOs để truyền dữ liệu giữa các layers

## 📝 Ví dụ

### Use Case (Command)
```csharp
public class CreateUserCommand
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

public class CreateUserCommandHandler
{
    private readonly IUserRepository _userRepository;
    
    public async Task<Guid> HandleAsync(CreateUserCommand command)
    {
        var user = new User(command.Username, command.Email);
        await _userRepository.AddAsync(user);
        return user.Id;
    }
}
```

### DTO
```csharp
public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}
```

