namespace API.Tests.Unit.ServiceTests
{
    public class AuthServiceTests
    {
        private readonly AuthService _authService;
        private readonly AppDbContext _dbContext;
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;

        public AuthServiceTests()
        {
            // Set up in-memory database options (if needed)
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("AuthTestDatabase")
                .Options;

            // Create the context and ensure it's fresh for each test
            _dbContext = new AppDbContext(_dbContextOptions);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            // Initialize the service with the real DbContext
            _authService = new AuthService(_dbContext);
        }

        [Fact]
        public void HashPassword_ReturnsHashedPassword()
        {
            // Arrange
            var password = "SecurePassword123!";

            // Act
            var hashedPassword = _authService.HashPassword(password);

            // Assert
            hashedPassword.Should().NotBeNullOrEmpty();
            hashedPassword.Should().NotBe(password); // Ensure it's different from the plain password
        }

        [Fact]
        public void HashPassword_GeneratesSameHashForSamePassword()
        {
            // Arrange
            var password = "SecurePassword123!";

            // Act
            var hashedPassword1 = _authService.HashPassword(password);
            var hashedPassword2 = _authService.HashPassword(password);

            // Assert
            hashedPassword1.Should().Be(hashedPassword2); // Same password should generate the same hash
        }

        [Theory]
        [InlineData("SecurePassword123!", "IncorrectPassword123!")] // should return false
        [InlineData("Password123!", "Password123!")] // should return true
        public void VerifyPassword_ReturnsExpectedResult(string storedPassword, string passwordToVerify)
        {
            // Arrange
            var storedHash = _authService.HashPassword(storedPassword);

            // Act
            var result = _authService.VerifyPassword(passwordToVerify, storedHash);

            // Assert
            if (storedPassword == passwordToVerify)
            {
                result.Should().BeTrue(); // Password matches
            }
            else
            {
                result.Should().BeFalse(); // Password does not match
            }
        }
    }
}
