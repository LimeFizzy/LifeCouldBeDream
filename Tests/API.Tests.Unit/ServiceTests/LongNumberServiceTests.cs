namespace API.Tests.Unit.ServiceTests
{
    public class LongNumberServiceTests
    {
        private readonly LongNumberService _longNumberService;
        private readonly AppDbContext _dbContext;
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;

        public LongNumberServiceTests()
        {
            // Set up in-memory database options
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("LongNumberTestDatabase")
                .Options;

            // Create the context and ensure it's fresh for each test
            _dbContext = new AppDbContext(_dbContextOptions);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            // Initialize the service with the real DbContext
            _longNumberService = new LongNumberService(_dbContext);
        }

        [Fact]
        public void GenerateSequence_ReturnsArrayOfCorrectLength()
        {
            // Arrange
            int level = 5;

            // Act
            var result = _longNumberService.GenerateSequence(level);

            // Assert
            result.Should().HaveCount(level);
        }

        [Fact]
        public void GenerateSequence_ReturnsNumbersInExpectedRange()
        {
            // Arrange
            int level = 10;

            // Act
            var result = _longNumberService.GenerateSequence(level);

            // Assert
            result.Should().OnlyContain(x => x >= 0 && x < 10);
        }

        [Theory]
        [InlineData(1, 0)]  // level = 1 should return score of 0
        [InlineData(2, 1)]  // level = 2 should return score of 1
        [InlineData(10, 9)] // level = 10 should return score of 9
        public void CalculateScore_ReturnsExpectedScore(int level, int expectedScore)
        {
            // Act
            var result = _longNumberService.CalculateScore(level);

            // Assert
            result.Should().Be(expectedScore);
        }
    }
}
