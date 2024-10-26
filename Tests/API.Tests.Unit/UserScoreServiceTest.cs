using API.Data;
using API.Models;
using API.Services;

namespace API.Tests.Unit
{
    public class UserScoreServiceTests
    {
        private readonly UserScoreService _userScoreService;
        private readonly AppDbContext _dbContext;
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;

        public UserScoreServiceTests()
        {
            // Set up in-memory database options
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            // Create the context and ensure it's fresh for each test
            _dbContext = new AppDbContext(_dbContextOptions);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            // Initialize the service with the real DbContext
            _userScoreService = new UserScoreService(_dbContext);
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
    }
}
