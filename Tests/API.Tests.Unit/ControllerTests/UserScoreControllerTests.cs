namespace API.Tests.Unit.ControllerTests
{
    public class UserScoreControllerTests
    {
        private readonly Mock<IUserScoreService> _mockUserScoreService;
        private readonly Mock<IUnifiedGamesService<int>> _mockUniServInt;
        private readonly Mock<IUnifiedGamesService<Square>> _mockUniServSquare;
        private readonly Mock<ILogger<UserScoreController>> _mockLogger;
        private readonly UserScoreController _controller;

        public UserScoreControllerTests()
        {
            _mockUserScoreService = new Mock<IUserScoreService>();
            _mockUniServInt = new Mock<IUnifiedGamesService<int>>();
            _mockUniServSquare = new Mock<IUnifiedGamesService<Square>>();
            _mockLogger = new Mock<ILogger<UserScoreController>>();

            _controller = new UserScoreController(
                _mockUserScoreService.Object,
                _mockUniServInt.Object,
                _mockUniServSquare.Object,
                _mockLogger.Object
            );
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenUserScoreServiceIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new UserScoreController(
                null,
                _mockUniServInt.Object,
                _mockUniServSquare.Object,
                _mockLogger.Object
            ));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenUniServIntIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new UserScoreController(
                _mockUserScoreService.Object,
                null,
                _mockUniServSquare.Object,
                _mockLogger.Object
            ));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenUniServSquareIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new UserScoreController(
                _mockUserScoreService.Object,
                _mockUniServInt.Object,
                null,
                _mockLogger.Object
            ));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new UserScoreController(
                _mockUserScoreService.Object,
                _mockUniServInt.Object,
                _mockUniServSquare.Object,
                null
            ));
        }

        #endregion

        #region GetLeaderboard Tests

        [Fact]
        public void GetLeaderboard_ReturnsOk_WhenScoresExistForGameType()
        {
            // Arrange
            var gameType = "testGame";
            var mockScores = new List<UserScore>
            {
                new UserScore { Username = "user1", Score = 100, GameType = gameType, GameDate = "2024/11/29" },
                new UserScore { Username = "user2", Score = 200, GameType = gameType, GameDate = "2024/11/28" }
            };

            _mockUserScoreService.Setup(service => service.GetLeaderboard())
                .Returns(mockScores);

            // Act
            var result = _controller.GetLeaderboard(gameType);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult?.Value as List<UserScore>;
            response.Should().NotBeNull().And.HaveCount(2);
            response[0].Score.Should().BeGreaterThan(response[1].Score); // Sorted by score
        }

        [Fact]
        public void GetLeaderboard_ReturnsServerError_WhenExceptionOccurs()
        {
            // Arrange
            var gameType = "testGame";
            _mockUserScoreService.Setup(service => service.GetLeaderboard())
                .Throws(new Exception("Database error"));

            // Act
            var result = _controller.GetLeaderboard(gameType);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var errorResult = result as ObjectResult;
            errorResult?.StatusCode.Should().Be(500);
        }

        #endregion

        #region SubmitScore Tests

        [Fact]
        public async Task SubmitScore_ReturnsOk_WhenScoreIsValid()
        {
            // Arrange
            var submission = new ScoreSubmission { Username = "user1", Level = 5 };
            var gameType = "testGame";
            _mockUserScoreService.Setup(service => service.GetGameTypeFromString(gameType))
                .Returns(GameTypes.SEQUENCE);
            _mockUniServSquare.Setup(service => service.CalculateScore(submission.Level))
                .Returns(100);

            // Act
            var result = await _controller.SubmitScore(submission, gameType);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SubmitScore_ReturnsBadRequest_WhenSubmissionIsInvalid()
        {
            // Arrange
            var submission = new ScoreSubmission { Username = "", Level = 5 };
            var gameType = "testGame";

            // Act
            var result = await _controller.SubmitScore(submission, gameType);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SubmitScore_ReturnsNotImplemented_WhenGameTypeNotImplemented()
        {
            // Arrange
            var submission = new ScoreSubmission { Username = "user1", Level = 5 };
            var gameType = "chimp"; // This game type is not implemented

            // Act
            var result = await _controller.SubmitScore(submission, gameType);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        #endregion

        #region DeleteScore Tests

        [Fact]
        public async Task DeleteScore_ReturnsOk_WhenScoreDeletedSuccessfully()
        {
            // Arrange
            int scoreId = 1;
            var userScore = new UserScore { Id = scoreId, Username = "user1", Score = 100, GameType = "testGame", GameDate = "2024/11/29" };
            _mockUserScoreService.Setup(service => service.GetScoreByIdAsync(scoreId))
                .ReturnsAsync(userScore);
            _mockUserScoreService.Setup(service => service.DeleteScoreAsync(userScore))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteScore(scoreId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteScore_ReturnsNotFound_WhenScoreDoesNotExist()
        {
            // Arrange
            int scoreId = 1;
            _mockUserScoreService.Setup(service => service.GetScoreByIdAsync(scoreId))
                .ReturnsAsync((UserScore)null);

            // Act
            var result = await _controller.DeleteScore(scoreId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task DeleteScore_ReturnsServerError_WhenExceptionOccurs()
        {
            // Arrange
            int scoreId = 1;
            _mockUserScoreService.Setup(service => service.GetScoreByIdAsync(scoreId))
                .Throws(new Exception("Database error"));

            // Act
            var result = await _controller.DeleteScore(scoreId);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var errorResult = result as ObjectResult;
            errorResult?.StatusCode.Should().Be(500);
        }

        #endregion
    }
}
