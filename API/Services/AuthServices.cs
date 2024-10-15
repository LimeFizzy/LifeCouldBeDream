using API.Data;
using System.Security.Cryptography;
using System.Text;

namespace API.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        // Method to register a new user
        public async Task<User> RegisterUserAsync(string username, string password)
        {
            // Check if the username is already taken
            var existingUser = await _context.Users.AnyAsync(u => u.Username == username);
            if (existingUser)
            {
                throw new ArgumentException("Username is already taken.");
            }

            // Hash the password
            var hashedPassword = HashPassword(password);

            // Create a new user object
            var user = new User
            {
                Username = username,
                PasswordHash = hashedPassword
            };

            // Add the user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        // Method to verify a user's password during login
        public bool VerifyPassword(string password, User user)
        {
            // Hash the provided password
            var hashedPassword = HashPassword(password);

            // Create a new User object with the hashed password to utilize CompareTo
            var comparisonUser = new User { Username = "", PasswordHash = hashedPassword };

            // Use CompareTo to compare with the user's stored password hash
            return comparisonUser.CompareTo(user) == 0; // Checks if the hashes are equal
        }

        // Utility method to hash a password using SHA-256
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
