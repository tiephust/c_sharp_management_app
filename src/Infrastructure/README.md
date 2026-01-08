# Infrastructure Layer

## 📋 Mô tả

Infrastructure Layer chứa các **implementations** của các interfaces được định nghĩa trong Domain và Application layers. Layer này xử lý các concerns về technical như database, external APIs, file system, etc.

## 📁 Cấu trúc thư mục

```
Infrastructure/
├── Data/              # Database access
│   ├── ApplicationDbContext.cs
│   ├── Repositories/ # Repository implementations
│   ├── Configurations/ # EF Core entity configurations
│   └── Migrations/   # Database migrations
├── Services/         # External service implementations
│   ├── Email/        # Email service
│   ├── Storage/      # File storage service
│   └── External/     # Third-party API integrations
├── Persistence/      # Persistence-specific implementations
└── Common/           # Infrastructure utilities
```

## 🎯 Nguyên tắc

- ✅ **Phụ thuộc vào** Domain và Application layers
- ✅ Implement các interfaces từ Domain/Application
- ✅ Chứa tất cả technical details (EF Core, HTTP clients, etc.)
- ✅ Có thể thay thế implementation mà không ảnh hưởng Domain/Application

## 📝 Ví dụ

### Repository Implementation
```csharp
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }
    
    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}
```

### Entity Configuration
```csharp
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "ManagementApp");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Username).IsRequired().HasMaxLength(50);
    }
}
```

