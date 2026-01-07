# 🔒 Security Guidelines - Hướng dẫn Bảo mật

## ⚠️ QUAN TRỌNG: Trước khi commit code

**LUÔN KIỂM TRA** các file sau trước khi commit để đảm bảo không commit các thông tin nhạy cảm:

---

## 🚫 Các File KHÔNG ĐƯỢC Commit

### 1. Configuration Files chứa Secrets

#### `appsettings.json` và `appsettings.Development.json`

- ❌ **KHÔNG commit** nếu chứa:

  - Connection strings với passwords
  - API keys
  - JWT secrets
  - OAuth client secrets
  - Email credentials
  - Third-party service keys

- ✅ **NÊN làm:**
  - Tạo `appsettings.Development.json.example` (template)
  - Sử dụng User Secrets cho development: `dotnet user-secrets`
  - Sử dụng Environment Variables cho production
  - Sử dụng Azure Key Vault / AWS Secrets Manager cho production

#### Ví dụ cấu trúc:

```json
// appsettings.Development.json.example (CÓ THỂ commit)
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ManagementApp;User=YOUR_USER;Password=YOUR_PASSWORD"
  },
  "JwtSettings": {
    "SecretKey": "YOUR_SECRET_KEY_HERE",
    "Issuer": "ManagementApp",
    "Audience": "ManagementApp"
  }
}

// appsettings.Development.json (KHÔNG commit file thật)
```

### 2. Keys và Certificates

- ❌ **KHÔNG commit:**
  - `*.pfx` (certificate files)
  - `*.key` (private keys)
  - `*.pem` (nếu chứa private keys)
  - `*.jks` (Java keystore)
  - `secrets.json` (nếu có)
  - `*.env` files với secrets

### 3. Database Files

- ❌ **KHÔNG commit:**
  - `*.mdf` (SQL Server database files)
  - `*.ldf` (SQL Server log files)
  - `*.db` (SQLite databases với real data)
  - Database backups `*.bak`

### 4. IDE và Editor Files

- ❌ **Đã được ignore** trong `.gitignore`:
  - `.vs/` (Visual Studio)
  - `.idea/` (JetBrains IDEs)
  - `*.user` files
  - `*.suo` files

### 5. Log Files

- ❌ **KHÔNG commit:**
  - Log files có thể chứa sensitive data
  - `*.log`
  - `logs/` directory

### 6. Temporary và Build Files

- ❌ **Đã được ignore** trong `.gitignore`:
  - `bin/`
  - `obj/`
  - `*.dll` (trừ khi là dependency)
  - `*.pdb` (debug symbols)

---

## ✅ Checklist trước khi Commit

Trước mỗi commit, hãy kiểm tra:

- [ ] Đã kiểm tra `appsettings.json` và `appsettings.Development.json`?
- [ ] Đã kiểm tra các file `.env` hoặc `secrets.json`?
- [ ] Đã kiểm tra các file certificate/keys?
- [ ] Đã kiểm tra log files?
- [ ] Đã kiểm tra database files?
- [ ] Đã chạy `git status` để xem các file sẽ được commit?
- [ ] Đã review `git diff` để đảm bảo không có secrets?

---

## 🛡️ Best Practices

### 1. Sử dụng User Secrets cho Development

```bash
# Khởi tạo user secrets
dotnet user-secrets init

# Thêm secret
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=ManagementApp;User=admin;Password=secret123"

# Xem secrets
dotnet user-secrets list
```

### 2. Sử dụng Environment Variables

```bash
# Windows PowerShell
$env:ConnectionStrings__DefaultConnection = "Server=localhost;Database=ManagementApp;User=admin;Password=secret123"

# Linux/Mac
export ConnectionStrings__DefaultConnection="Server=localhost;Database=ManagementApp;User=admin;Password=secret123"
```

### 3. Sử dụng Azure Key Vault (Production)

```csharp
// Trong Program.cs
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
    new DefaultAzureCredential());
```

### 4. Tạo Template Files

Luôn tạo file `.example` hoặc `.template` cho các file config:

```
appsettings.Development.json.example
appsettings.Production.json.example
.env.example
```

---

## 🔍 Cách Kiểm tra Secrets trong Code

### Sử dụng Git Hooks (Pre-commit)

Tạo file `.git/hooks/pre-commit`:

```bash
#!/bin/sh
# Kiểm tra các patterns phổ biến của secrets
if git diff --cached | grep -E "(password|secret|key|token)\s*[:=]\s*['\"][^'\"]+['\"]"; then
    echo "⚠️  WARNING: Possible secret detected in staged files!"
    echo "Please review before committing."
    exit 1
fi
```

### Sử dụng Tools

- **git-secrets** (AWS): Scan for AWS keys
- **truffleHog**: Scan for secrets in git history
- **gitleaks**: Scan for secrets and keys

---

## 📝 Common Secrets Patterns cần tránh

Các patterns sau thường là secrets và không nên commit:

- `password\s*[:=]\s*['"][^'"]+['"]`
- `secret\s*[:=]\s*['"][^'"]+['"]`
- `api[_-]?key\s*[:=]\s*['"][^'"]+['"]`
- `access[_-]?token\s*[:=]\s*['"][^'"]+['"]`
- `connection[_-]?string\s*[:=]\s*['"][^'"]+['"]`
- `jwt[_-]?secret\s*[:=]\s*['"][^'"]+['"]`
- `oauth[_-]?secret\s*[:=]\s*['"][^'"]+['"]`

---

## 🚨 Nếu đã Commit Secrets

Nếu bạn đã vô tình commit secrets:

1. **Ngay lập tức:**

   - Xóa secrets khỏi code
   - Thay đổi secrets đó (nếu có thể)
   - Revoke API keys/tokens đã commit

2. **Xóa khỏi Git History:**

   ```bash
   # Xóa file khỏi history (cẩn thận!)
   git filter-branch --force --index-filter \
     "git rm --cached --ignore-unmatch appsettings.Development.json" \
     --prune-empty --tag-name-filter cat -- --all

   # Force push (chỉ nếu bạn chắc chắn!)
   git push origin --force --all
   ```

3. **Sử dụng BFG Repo-Cleaner** (an toàn hơn):
   ```bash
   bfg --delete-files appsettings.Development.json
   git reflog expire --expire=now --all
   git gc --prune=now --aggressive
   ```

---

## 📚 Tài liệu Tham khảo

- [OWASP - Secrets Management](https://owasp.org/www-project-web-security-testing-guide/latest/4-Web_Application_Security_Testing/02-Configuration_and_Deployment_Management_Testing/10-Test_for_Backup_and_Unreferenced_Files)
- [Microsoft - Safe storage of app secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [GitHub - Removing sensitive data](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/removing-sensitive-data-from-a-repository)

---

## 💡 Nhắc nhở

**Nhớ rằng:** Một khi đã commit secrets lên Git (đặc biệt là public repository), coi như secrets đó đã bị lộ. Luôn luôn:

- ✅ Kiểm tra kỹ trước khi commit
- ✅ Sử dụng tools để scan secrets
- ✅ Sử dụng User Secrets / Environment Variables
- ✅ Tạo template files thay vì commit file thật
- ✅ Review code trước khi push

**Khi nghi ngờ, đừng commit!**
