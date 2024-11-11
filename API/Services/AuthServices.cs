using API.Data;
using API.Interfaces;
using System.Security.Cryptography;
using System.Text;
using API.Exceptions;

namespace API.Services;

public class AuthService() : IAuthService
{
    public string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    public bool VerifyPassword(string password, string storedHash)
    {
        var hashedPassword = HashPassword(password);
        return hashedPassword == storedHash;
    }

    public void ValidatePasswordStrength(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 4)
        {
            throw new WeakPasswordException("Password must be at least 4 characters long.");
        }

        if (!password.Any(char.IsUpper))
        {
            throw new WeakPasswordException("Password must contain at least one uppercase letter.");
        }

        if (!password.Any(char.IsLower))
        {
            throw new WeakPasswordException("Password must contain at least one lowercase letter.");
        }

        if (!password.Any(char.IsDigit))
        {
            throw new WeakPasswordException("Password must contain at least one digit.");
        }

        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            throw new WeakPasswordException("Password must contain at least one special character.");
        }
    }
}