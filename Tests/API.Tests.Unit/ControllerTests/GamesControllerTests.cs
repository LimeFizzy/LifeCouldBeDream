namespace API.Tests.Unit.ControllerTests
{
    public class GamesControllerTests
    {
        private readonly AppDbContext _context;
        private readonly GamesController _controller;

        public GamesControllerTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "GamesControllerDatabase")
                .Options;

            _context = new AppDbContext(options);

            // Seed the database with test data
            _context.Games.RemoveRange(_context.Games);
            _context.SaveChanges();

            _context.Games.AddRange(
                new GameDTO { GameID = 1, Title = "Test Game 1", Description = "Description 1", Icon = "Icon1", AltText = "Alt 1", Route = "Route1" },
                new GameDTO { GameID = 2, Title = "Test Game 2", Description = "Description 2", Icon = "Icon2", AltText = "Alt 2", Route = "Route2" }
            );
            _context.SaveChanges();

            _controller = new GamesController(_context);
        }

        public void Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
        }

        [Fact]
        public async Task GetGames_ReturnsAllGames()
        {
            // Act
            var result = await _controller.GetGames();

            // Assert
            result.Should().BeOfType<ActionResult<IEnumerable<GameDTO>>>();

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();

            var games = okResult?.Value as List<GameDTO>;
            games.Should().NotBeNull();
            games.Should().HaveCount(2);
            games[0].Title.Should().Be("Test Game 1");
            games[1].Title.Should().Be("Test Game 2");
        }

        [Fact]
        public async Task AddGame_ValidGame_ReturnsCreatedAtAction()
        {
            // Arrange
            var newGame = new GameDTO { GameID = 3, Title = "Test Game 3", Description = "Description 3", Icon = "icon3.png", AltText = "Alt text 3", Route = "/test-game-3" };

            // Act
            var result = await _controller.AddGame(newGame);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();

            var createdGame = createdResult?.Value as GameDTO;
            createdGame.Should().NotBeNull();
            createdGame!.Title.Should().Be("Test Game 3");
        }

        [Fact]
        public async Task AddGame_NullGame_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.AddGame(null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task AddGame_InvalidGame_ReturnsBadRequest()
        {
            // Arrange
            var invalidGame = new GameDTO { GameID = 4, Title = "", Description = "", Icon = "", AltText = "", Route = "" };

            // Act
            var result = await _controller.AddGame(invalidGame);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
        }
    }
}
