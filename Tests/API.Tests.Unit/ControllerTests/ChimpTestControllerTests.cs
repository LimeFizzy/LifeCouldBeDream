namespace API.Tests.Unit.ControllerTests
{
    public class ChimpTestControllerTests
    {
        private readonly Mock<IUnifiedGamesService<SquareChimp>> _mockUniServ;
        private readonly Mock<ILogger<ChimpTestController>> _mockLogger;
        private readonly ChimpTestController _controller;

        public ChimpTestControllerTests()
        {
            _mockUniServ = new Mock<IUnifiedGamesService<SquareChimp>>();
            _mockLogger = new Mock<ILogger<ChimpTestController>>();

            _controller = new ChimpTestController(_mockLogger.Object, _mockUniServ.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
        {
            // Act & Assert
            _ = Assert.Throws<ArgumentNullException>(() => new ChimpTestController(null, _mockUniServ.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenServiceIsNull()
        {
            // Act & Assert
            _ = Assert.Throws<ArgumentNullException>(() => new ChimpTestController(_mockLogger.Object, null));
        }

        [Fact]
        public void GenerateSequence_ReturnsOk_WithValidLevel()
        {
            // Arrange
            int level = 3;
            var expectedSequence = new SquareChimp[]
            {
                new SquareChimp(1, false, 0, 0),
                new SquareChimp(2, false, 1, 0),
                new SquareChimp(3, false, 2, 0)
            };

            _mockUniServ.Setup(serv => serv.GenerateSequence(level)).Returns(expectedSequence);

            // Act
            var result = _controller.GenerateSequence(level);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedSequence);
        }

        [Fact]
        public void GenerateSequence_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            int level = 2;
            _mockUniServ.Setup(serv => serv.GenerateSequence(level)).Throws(new Exception("Test error"));

            // Act
            var result = _controller.GenerateSequence(level);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult!.StatusCode.Should().Be(500);
        }

        [Fact]
        public void GenerateSequence_ReturnsInternalServerError_WhenArgumentOutOfRangeExceptionIsThrown()
        {
            // Arrange
            int invalidLevel = 0;
            _mockUniServ.Setup(serv => serv.GenerateSequence(invalidLevel))
                .Throws(new ArgumentOutOfRangeException("level", "Level must be greater than zero."));

            // Act
            var result = _controller.GenerateSequence(invalidLevel);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult!.StatusCode.Should().Be(500);
        }
    }
}
