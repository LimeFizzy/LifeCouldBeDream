namespace API.Tests.Unit.ControllerTests
{
    public class LongNumberControllerTests
    {
        private readonly Mock<IUnifiedGamesService<int>> _mockUniServ;
        private readonly Mock<ILogger<LongNumberController>> _mockLogger;
        private readonly LongNumberController _controller;

        public LongNumberControllerTests()
        {
            // Mock the IUnifiedGamesService<int> and ILogger<LongNumberController>
            _mockUniServ = new Mock<IUnifiedGamesService<int>>();
            _mockLogger = new Mock<ILogger<LongNumberController>>();

            // Initialize the controller with mocked dependencies
            _controller = new LongNumberController(_mockLogger.Object, _mockUniServ.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new LongNumberController(null, _mockUniServ.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenServiceIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new LongNumberController(_mockLogger.Object, null));
        }

        [Fact]
        public void GenerateSequence_ReturnsOk_WithValidLevel()
        {
            // Arrange
            int level = 3;
            var expectedSequence = new[] { 1, 2, 3 };  // Example sequence for level 3
            int timeLimit = 3 + level - 1;  // Time limit based on the level
            _mockUniServ.Setup(serv => serv.GenerateSequence(level)).Returns(expectedSequence);

            // Act
            var result = _controller.GenerateSequence(level);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
        }

        [Fact]
        public void GenerateSequence_ReturnsBadRequest_WhenExceptionIsThrown()
        {
            // Arrange
            int level = 2;
            _mockUniServ.Setup(serv => serv.GenerateSequence(level)).Throws(new Exception("Test error"));

            // Act
            var result = _controller.GenerateSequence(level);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
        }

        [Fact]
        public void GenerateSequence_ReturnsBadRequest_WhenInvalidLevel()
        {
            // Arrange
            int level = -1;  // Invalid level
            _mockUniServ.Setup(serv => serv.GenerateSequence(level)).Throws(new ArgumentOutOfRangeException("Invalid level"));

            // Act
            var result = _controller.GenerateSequence(level);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
        }
    }
}
