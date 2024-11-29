namespace API.Tests.Unit.ServiceTests
{
    public class UserScoreServiceTests
    {
        private readonly UserScoreService _userScoreService;
        private readonly AppDbContext _dbContext;
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;
        private readonly ILogger<UserScoreService> _logger;

        public UserScoreServiceTests()
        {
            // Set up in-memory database options
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("UserScoreTestDatabase")
                .Options;

            // Create the context and ensure it's fresh for each test
            _dbContext = new AppDbContext(_dbContextOptions);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console() // or .WriteTo.File("log.txt")
                .CreateLogger();

            // Wrap Serilog in Microsoft.Extensions.Logging.ILogger
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            });
            _logger = loggerFactory.CreateLogger<UserScoreService>();

            // Initialize the service with the real DbContext
            _userScoreService = new UserScoreService(_dbContext, _logger);
        }

        [Fact]
        public void GetLeaderboard_ReturnsAllUserScores()
        {
            // Arrange
            var userScores = new List<UserScore>
            {
                new UserScore { Id = 1, Score = 100, Username = "User1", GameDate = "2024-12-31" },
                new UserScore { Id = 2, Score = 200, Username = "User2", GameDate = "2024-12-31" }
            };

            _dbContext.UserScores.AddRange(userScores);
            _dbContext.SaveChanges();

            // Act
            var result = _userScoreService.GetLeaderboard();

            // Assert
            result.Should().BeEquivalentTo(userScores);
        }

        [Fact]
        public void GetLeaderboard_ReturnsEmpty_WhenNoScores()
        {
            // Act
            var result = _userScoreService.GetLeaderboard();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task SaveScoreAsync_SavesUserScore()
        {
            // Arrange
            var userScore = new UserScore { Score = 150, Username = "User1", GameDate = "2024-12-31" };

            // Act
            await _userScoreService.SaveScoreAsync(userScore);

            // Assert
            var savedScore = await _dbContext.UserScores.FindAsync(userScore.Id);
            savedScore.Should().BeEquivalentTo(userScore);
        }

        [Fact]
        public async Task SaveScoreAsync_ThrowsArgumentNullException_WhenUserScoreIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _userScoreService.SaveScoreAsync(null));
        }

        [Theory]
        [InlineData("longNumberMemory", GameTypes.LONG_NUMBER)]
        [InlineData("sequenceMemory", GameTypes.SEQUENCE)]
        [InlineData("chimpTest", GameTypes.CHIMP)]
        [InlineData("invalidGameType", null)]
        public void GetGameTypeFromString_ReturnsCorrectGameType(string input, GameTypes? expected)
        {
            // Act
            var result = _userScoreService.GetGameTypeFromString(input);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public async Task GetScoreByIdAsync_ReturnsCorrectScore()
        {
            // Arrange
            var userScore = new UserScore { Score = 150, Username = "User1", GameDate = "2024-12-31" };
            _dbContext.UserScores.Add(userScore);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userScoreService.GetScoreByIdAsync(userScore.Id);

            // Assert
            result.Should().BeEquivalentTo(userScore);
        }

        [Fact]
        public async Task GetScoreByIdAsync_ThrowsArgumentNullException_WhenScoreNotFound()
        {
            // Act & Assert
            await Assert.ThrowsAsync<IOException>(() => _userScoreService.GetScoreByIdAsync(999));
        }

        [Fact]
        public async Task GetScoreByIdAsync_ThrowsArgumentException_WhenInvalidId()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _userScoreService.GetScoreByIdAsync(0));
            await Assert.ThrowsAsync<ArgumentException>(() => _userScoreService.GetScoreByIdAsync(-1));
        }

        [Fact]
        public async Task DeleteScoreAsync_DeletesScore()
        {
            // Arrange
            var userScore = new UserScore { Score = 150, Username = "User1", GameDate = "2024-12-31" };
            _dbContext.UserScores.Add(userScore);
            await _dbContext.SaveChangesAsync();

            // Act
            await _userScoreService.DeleteScoreAsync(userScore);

            // Assert
            var deletedScore = await _dbContext.UserScores.FindAsync(userScore.Id);
            deletedScore.Should().BeNull();
        }

        [Fact]
        public async Task DeleteScoreAsync_ThrowsArgumentNullException_WhenScoreIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _userScoreService.DeleteScoreAsync(null));
        }

        [Fact]
        public async Task DeleteScoreAsync_ThrowsException_WhenScoreNotFound()
        {
            // Arrange
            var userScore = new UserScore { Id = 999, Score = 150, Username = "NonExistingUser", GameDate = "2024-12-31" };

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => _userScoreService.DeleteScoreAsync(userScore));
        }
    }
}
