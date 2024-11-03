using API.Data;
using API.Interfaces;
using System.Security.Cryptography;
using System.Text;

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
}