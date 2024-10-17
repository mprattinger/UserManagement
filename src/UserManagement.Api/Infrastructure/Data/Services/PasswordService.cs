using Microsoft.AspNetCore.Identity;

namespace UserManagement.Api.Infrastructure.Data.Services;

public interface IPasswordService
{
    string HashPassword(string password);
    PasswordVerificationResult VerifyPassword(string hashedPassword, string providedPassword);
}

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<string> _passwordHasher = new PasswordHasher<string>();

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword("", password);
    }

    public PasswordVerificationResult VerifyPassword(string hashedPassword, string providedPassword)
    {
        return _passwordHasher.VerifyHashedPassword("", hashedPassword, providedPassword);
    }
}
