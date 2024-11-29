namespace API.Tests.Unit.ControllerTests
{
    public class SequenceControllerTests
    {
        private readonly Mock<IUnifiedGamesService<Square>> _mockUniServ;
        private readonly Mock<ILogger<SequenceController>> _mockLogger;
        private readonly SequenceController _controller;

        public SequenceControllerTests()
        {
            // Mock the IUnifiedGamesService<Square> and ILogger<SequenceController>
            _mockUniServ = new Mock<IUnifiedGamesService<Square>>();
            _mockLogger = new Mock<ILogger<SequenceController>>();

            // Initialize the controller with mocked dependencies
            _controller = new SequenceController(_mockLogger.Object, _mockUniServ.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SequenceController(null, _mockUniServ.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenServiceIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SequenceController(_mockLogger.Object, null));
        }

        [Fact]
        public void GenerateSequence_ReturnsOk_WithValidLevel()
        {
            // Arrange
            int level = 3;
            var expectedSequence = new[] { new Square(1, true), new Square(2, true), new Square(3, true) };  // Example sequence for level 3
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

        [Fact]
        public void GenerateSequence_ReturnsBadRequest_WhenLevelIsZero()
        {
            // Arrange
            int level = 0;  // Edge case, level 0
            _mockUniServ.Setup(serv => serv.GenerateSequence(level)).Throws(new ArgumentOutOfRangeException("Level cannot be zero"));

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
