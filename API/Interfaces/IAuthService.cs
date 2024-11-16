namespace API.Interfaces
{
    public interface IAuthService
    {
        void ValidatePasswordStrength(string password);
        string HashPassword(string password);
        bool VerifyPassword(string password, string storedHash);
    }
}
