# Presentation Layer

## 📋 Mô tả

Presentation Layer là lớp ngoài cùng, chịu trách nhiệm giao tiếp với người dùng hoặc hệ thống bên ngoài. Có thể là Web API, Console Application, gRPC, etc.

## 📁 Cấu trúc thư mục

```
Presentation/
├── Console/          # Console application (hiện tại)
│   └── Program.cs
├── API/              # Web API (tương lai)
│   ├── Controllers/
│   ├── Middleware/
│   └── Filters/
└── Common/           # Presentation utilities
```

## 🎯 Nguyên tắc

- ✅ **Phụ thuộc vào** Application Layer
- ✅ **KHÔNG** có business logic
- ✅ Chỉ điều phối requests đến Application layer
- ✅ Xử lý HTTP, Console I/O, validation input

## 📝 Ví dụ

### Console Command Handler
```csharp
public class ConsoleUserService
{
    private readonly CreateUserCommandHandler _createUserHandler;
    
    public async Task CreateUserAsync()
    {
        Console.Write("Username: ");
        var username = Console.ReadLine();
        
        var command = new CreateUserCommand { Username = username };
        var userId = await _createUserHandler.HandleAsync(command);
        
        Console.WriteLine($"User created with ID: {userId}");
    }
}
```

### API Controller (tương lai)
```csharp
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly CreateUserCommandHandler _createUserHandler;
    
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserCommand command)
    {
        var userId = await _createUserHandler.HandleAsync(command);
        return Ok(new { Id = userId });
    }
}
```

