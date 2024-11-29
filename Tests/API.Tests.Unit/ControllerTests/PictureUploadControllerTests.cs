namespace API.Tests.Unit.ControllerTests
{
    public class PictureUploadControllerTests
    {
        private readonly Mock<IPictureUploadService> _mockPictureUploadService;
        private readonly Mock<ILogger<PictureUploadController>> _mockLogger;
        private readonly PictureUploadController _controller;

        public PictureUploadControllerTests()
        {
            _mockPictureUploadService = new Mock<IPictureUploadService>();
            _mockLogger = new Mock<ILogger<PictureUploadController>>();
            _controller = new PictureUploadController(
                _mockPictureUploadService.Object,
                _mockLogger.Object
            );
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenUserServiceIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new PictureUploadController(
                null,
                _mockLogger.Object
            ));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new PictureUploadController(
                _mockPictureUploadService.Object,
                null
            ));
        }

        #endregion

        #region UploadProfileImage Tests

        [Fact]
        public async Task UploadProfileImage_ReturnsOk_WhenImageIsUploadedSuccessfully()
        {
            // Arrange
            var userId = 1;
            var file = new Mock<IFormFile>();
            file.Setup(f => f.Length).Returns(1000);
            var expectedFilePath = "/path/to/file.jpg";

            _mockPictureUploadService.Setup(service => service.UploadProfileImageAsync(userId, file.Object))
                .ReturnsAsync(expectedFilePath);

            // Act
            var result = await _controller.UploadProfileImage(userId, file.Object);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task UploadProfileImage_ReturnsBadRequest_WhenProfileImageIsNullOrEmpty()
        {
            // Arrange
            var userId = 1;
            IFormFile file = null;

            // Act
            var result = await _controller.UploadProfileImage(userId, file);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UploadProfileImage_ReturnsForbidden_WhenUnauthorizedAccessExceptionIsThrown()
        {
            // Arrange
            var userId = 1;
            var file = new Mock<IFormFile>();
            _mockPictureUploadService.Setup(service => service.UploadProfileImageAsync(userId, file.Object))
                .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

            // Act
            var result = await _controller.UploadProfileImage(userId, file.Object);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var objectResult = result as BadRequestObjectResult;
            objectResult?.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task UploadProfileImage_ReturnsBadRequest_WhenArgumentExceptionIsThrown()
        {
            // Arrange
            var userId = 1;
            var file = new Mock<IFormFile>();
            _mockPictureUploadService.Setup(service => service.UploadProfileImageAsync(userId, file.Object))
                .ThrowsAsync(new ArgumentException("Invalid argument"));

            // Act
            var result = await _controller.UploadProfileImage(userId, file.Object);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UploadProfileImage_ReturnsNotFound_WhenKeyNotFoundExceptionIsThrown()
        {
            // Arrange
            var userId = 1;
            var file = new Mock<IFormFile>();
            _mockPictureUploadService.Setup(service => service.UploadProfileImageAsync(userId, file.Object))
                .ThrowsAsync(new KeyNotFoundException("User not found"));

            // Act
            var result = await _controller.UploadProfileImage(userId, file.Object);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UploadProfileImage_ReturnsServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var userId = 1;
            var file = new Mock<IFormFile>();
            _mockPictureUploadService.Setup(service => service.UploadProfileImageAsync(userId, file.Object))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.UploadProfileImage(userId, file.Object);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        #endregion

        #region GetProfileImage Tests

        [Fact]
        public async Task GetProfileImage_ReturnsFile_WhenProfileImageExists()
        {
            // Arrange
            var userId = 1;
            var fileBytes = new byte[] { 1, 2, 3 }; // Dummy file data
            _mockPictureUploadService.Setup(service => service.GetProfileImageAsync(userId))
                .ReturnsAsync(fileBytes);

            // Act
            var result = await _controller.GetProfileImage(userId);

            // Assert
            result.Should().BeOfType<FileContentResult>();
            var fileResult = result as FileContentResult;
            fileResult?.ContentType.Should().Be("image/jpeg");
            fileResult?.FileContents.Should().BeEquivalentTo(fileBytes);
        }

        [Fact]
        public async Task GetProfileImage_ReturnsNotFound_WhenProfileImageNotFound()
        {
            // Arrange
            var userId = 1;
            _mockPictureUploadService.Setup(service => service.GetProfileImageAsync(userId))
                .ReturnsAsync((byte[])null);

            // Act
            var result = await _controller.GetProfileImage(userId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetProfileImage_ReturnsForbidden_WhenUnauthorizedAccessExceptionIsThrown()
        {
            // Arrange
            var userId = 1;
            _mockPictureUploadService.Setup(service => service.GetProfileImageAsync(userId))
                .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

            // Act
            var result = await _controller.GetProfileImage(userId);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult?.StatusCode.Should().Be(403);
        }

        [Fact]
        public async Task GetProfileImage_ReturnsServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var userId = 1;
            _mockPictureUploadService.Setup(service => service.GetProfileImageAsync(userId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetProfileImage(userId);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult?.StatusCode.Should().Be(500);
        }

        #endregion
    }
}
