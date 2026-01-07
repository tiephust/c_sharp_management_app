using ManagementApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Configure PostgreSQL Database
        var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
        
        // Debug: Hiển thị thông tin cấu hình
        var environment = context.HostingEnvironment.EnvironmentName;
        Console.WriteLine($"🔧 Environment: {environment}");
        Console.WriteLine($"📁 Đang đọc từ: appsettings.json và appsettings.{environment}.json");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            Console.WriteLine("⚠️ CẢNH BÁO: Connection string trống!");
        }
        else
        {
            // Ẩn password trong log
            var safeConnectionString = connectionString.Contains("Password=") 
                ? connectionString.Substring(0, connectionString.IndexOf("Password=")) + "Password=***"
                : connectionString;
            Console.WriteLine($"🔗 Connection String: {safeConnectionString}");
        }
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "ManagementApp");
            }));
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

