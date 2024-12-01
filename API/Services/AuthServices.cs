using System.Text;
using API.Exceptions;
using API.Interfaces;
using System.Security.Cryptography;

namespace API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;

        public AuthService(ILogger<AuthService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("HashPassword called with null or empty password.");
                throw new ArgumentNullException(nameof(password), "Password cannot be null or empty.");
            }

            try
            {
                using (var sha256 = SHA256.Create())
                {
                    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                    return Convert.ToBase64String(hashedBytes);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while hashing the password.");
                throw;
            }
        }

        public bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("VerifyPassword called with null or empty password.");
                throw new ArgumentNullException(nameof(password), "Password cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(storedHash))
            {
                _logger.LogWarning("VerifyPassword called with null or empty stored hash.");
                throw new ArgumentNullException(nameof(storedHash), "Stored hash cannot be null or empty.");
            }

            try
            {
                var hashedPassword = HashPassword(password);
                return hashedPassword == storedHash;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while verifying the password.");
                throw;
            }
        }

        public void ValidatePasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("ValidatePasswordStrength called with null or empty password.");
                throw new WeakPasswordException("Password cannot be null or empty.");
            }


            if (password.Length < 4)
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
}
