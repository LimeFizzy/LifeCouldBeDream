namespace API.Tests.Unit.ServiceTests
{
    public class AuthServiceTests
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthService> _logger;

        public AuthServiceTests()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            });
            _logger = loggerFactory.CreateLogger<AuthService>();

            _authService = new AuthService(_logger);
        }

        [Fact]
        public void HashPassword_ReturnsHashedPassword()
        {
            var password = "SecurePassword123!";

            var hashedPassword = _authService.HashPassword(password);

            hashedPassword.Should().NotBeNullOrEmpty();
            hashedPassword.Should().NotBe(password);
        }

        [Fact]
        public void HashPassword_GeneratesSameHashForSamePassword()
        {
            var password = "SecurePassword123!";

            var hashedPassword1 = _authService.HashPassword(password);
            var hashedPassword2 = _authService.HashPassword(password);

            hashedPassword1.Should().Be(hashedPassword2);
        }

        [Fact]
        public void HashPassword_ThrowsException_WhenPasswordIsNullOrEmpty()
        {
            Action actNull = () => _authService.HashPassword(null);
            Action actEmpty = () => _authService.HashPassword(string.Empty);

            actNull.Should().Throw<ArgumentNullException>().WithMessage("*password*");
            actEmpty.Should().Throw<ArgumentNullException>().WithMessage("*password*");
        }

        [Theory]
        [InlineData("SecurePassword123!", "IncorrectPassword123!")]
        [InlineData("Password123!", "Password123!")]
        public void VerifyPassword_ReturnsExpectedResult(string storedPassword, string passwordToVerify)
        {
            var storedHash = _authService.HashPassword(storedPassword);

            var result = _authService.VerifyPassword(passwordToVerify, storedHash);

            if (storedPassword == passwordToVerify)
            {
                result.Should().BeTrue();
            }
            else
            {
                result.Should().BeFalse();
            }
        }

        [Fact]
        public void VerifyPassword_ThrowsException_WhenInputIsInvalid()
        {
            var storedHash = _authService.HashPassword("ValidPassword123!");

            Action actNullPassword = () => _authService.VerifyPassword(null, storedHash);
            Action actEmptyPassword = () => _authService.VerifyPassword(string.Empty, storedHash);
            Action actNullHash = () => _authService.VerifyPassword("ValidPassword123!", null);
            Action actEmptyHash = () => _authService.VerifyPassword("ValidPassword123!", string.Empty);

            actNullPassword.Should().Throw<ArgumentNullException>().WithMessage("*password*");
            actEmptyPassword.Should().Throw<ArgumentNullException>().WithMessage("*password*");
            actNullHash.Should().Throw<ArgumentNullException>().WithMessage("*storedHash*");
            actEmptyHash.Should().Throw<ArgumentNullException>().WithMessage("*storedHash*");
        }

        [Theory]
        [InlineData("abc", "Password must be at least 4 characters long.")]
        [InlineData("abcde", "Password must contain at least one uppercase letter.")]
        [InlineData("ABCDE", "Password must contain at least one lowercase letter.")]
        [InlineData("Abcde", "Password must contain at least one digit.")]
        [InlineData("Abcde1", "Password must contain at least one special character.")]
        public void ValidatePasswordStrength_ThrowsWeakPasswordException_WhenPasswordIsWeak(string password, string expectedMessage)
        {
            Action act = () => _authService.ValidatePasswordStrength(password);

            act.Should().Throw<WeakPasswordException>().WithMessage(expectedMessage);
        }

        [Fact]
        public void ValidatePasswordStrength_Passes_ForStrongPassword()
        {
            var strongPassword = "ValidPassword123!";

            Action act = () => _authService.ValidatePasswordStrength(strongPassword);

            act.Should().NotThrow();
        }

        [Fact]
        public void ValidatePasswordStrength_ThrowsException_WhenPasswordIsNullOrEmpty()
        {
            Action actNull = () => _authService.ValidatePasswordStrength(null);
            Action actEmpty = () => _authService.ValidatePasswordStrength(string.Empty);

            actNull.Should().Throw<WeakPasswordException>().WithMessage("Password cannot be null or empty.");
            actEmpty.Should().Throw<WeakPasswordException>().WithMessage("Password cannot be null or empty.");
        }
    }
}
