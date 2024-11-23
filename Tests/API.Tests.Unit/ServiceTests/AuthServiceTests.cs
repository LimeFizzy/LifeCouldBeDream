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
    }
}
