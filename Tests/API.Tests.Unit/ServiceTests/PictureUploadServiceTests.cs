namespace API.Tests.Unit.ServiceTests
{
    public class PictureUploadServiceTests
    {
        private readonly PictureUploadService _pictureUploadService;
        private readonly AppDbContext _dbContext;
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;
        private readonly ILogger<PictureUploadService> _logger;

        public PictureUploadServiceTests()
        {
            // Set up in-memory database options
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("PictureUploadTestDatabase")
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
            _logger = loggerFactory.CreateLogger<PictureUploadService>();

            // Initialize the service with the real DbContext
            _pictureUploadService = new PictureUploadService(_dbContext, _logger);
        }

        [Fact]
        public async Task UploadProfileImageAsync_SavesImageAndPath()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                UserId = userId,
                Username = "User1",
                PasswordHash = "hashedpassword"
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var fileMock = new Mock<IFormFile>();
            var fileName = "profile.jpg";
            var fileContent = "This is a test file.";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            await writer.WriteAsync(fileContent);
            await writer.FlushAsync();
            stream.Position = 0;

            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Callback<Stream, CancellationToken>((s, c) => stream.CopyTo(s));

            // Act
            var result = await _pictureUploadService.UploadProfileImageAsync(userId, fileMock.Object);

            // Assert
            var savedUser = await _dbContext.Users.FindAsync(userId);
            savedUser.ProfileImagePath.Should().StartWith(Path.Combine(Directory.GetCurrentDirectory(), "uploads"));
            result.Should().StartWith(savedUser.ProfileImagePath);
        }

        [Fact]
        public async Task GetProfileImageAsync_ReturnsImageBytes()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                UserId = userId,
                Username = "User1",
                PasswordHash = "hashedpassword",
                ProfileImagePath = Path.Combine("uploads", "image.jpg")
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Generate file content
            var fileContent = Enumerable.Range(0, 256).Select(b => (byte)b).ToArray();

            // Ensure the uploads directory exists
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, "image.jpg");
            try
            {
                // Simulate file existence
                await File.WriteAllBytesAsync(filePath, fileContent);

                // Act
                var result = await _pictureUploadService.GetProfileImageAsync(userId);

                // Assert
                result.Should().NotBeNullOrEmpty();
                result.Should().BeEquivalentTo(fileContent);
            }
            finally
            {
                // Cleanup
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }


        [Fact]
        public async Task UploadProfileImageAsync_ThrowsException_WhenNoFileUploaded()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                UserId = userId,
                Username = "User1",
                PasswordHash = "hashedpassword"
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            Func<Task> act = async () => await _pictureUploadService.UploadProfileImageAsync(userId, null);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("No file uploaded.");
        }

        [Fact]
        public async Task GetProfileImageAsync_ThrowsException_WhenUserNotFound()
        {
            // Act
            Func<Task> act = async () => await _pictureUploadService.GetProfileImageAsync(999);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("User with ID 999 or their profile image was not found.");
        }

        [Fact]
        public async Task UploadProfileImageAsync_CreatesUploadsDirectory_WhenNotExisting()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                UserId = userId,
                Username = "User1",
                PasswordHash = "hashedpassword"
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var fileMock = new Mock<IFormFile>();
            var fileName = "profile.jpg";
            var fileContent = "This is a test file.";
            var stream = new MemoryStream();
            await new StreamWriter(stream).WriteAsync(fileContent);
            stream.Position = 0;

            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (Directory.Exists(uploadsFolder))
            {
                Directory.Delete(uploadsFolder, true);
            }

            // Act
            var result = await _pictureUploadService.UploadProfileImageAsync(userId, fileMock.Object);

            // Assert
            Directory.Exists(uploadsFolder).Should().BeTrue();
            File.Exists(result).Should().BeTrue();

            // Cleanup
            Directory.Delete(uploadsFolder, true);
        }

        [Fact]
        public async Task GetProfileImageAsync_ThrowsFileNotFoundException_WhenImageNotFound()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                UserId = userId,
                Username = "User1",
                PasswordHash = "hashedpassword",
                ProfileImagePath = "nonexistent/path.jpg"
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            Func<Task> act = async () => await _pictureUploadService.GetProfileImageAsync(userId);

            // Assert
            await act.Should().ThrowAsync<FileNotFoundException>()
                .WithMessage("Image file not found.");
        }

        [Fact]
        public async Task UploadProfileImageAsync_ThrowsException_WhenUserNotFound()
        {
            // Arrange
            var userId = 999;
            var fileMock = new Mock<IFormFile>();

            // Act
            Func<Task> act = async () => await _pictureUploadService.UploadProfileImageAsync(userId, fileMock.Object);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"User with ID {userId} not found.");
        }

        [Fact]
        public async Task GetProfileImageAsync_ReturnsNull_WhenProfileImagePathIsEmpty()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                UserId = userId,
                Username = "User1",
                PasswordHash = "hashedpassword",
                ProfileImagePath = string.Empty
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            Func<Task> act = async () => await _pictureUploadService.GetProfileImageAsync(userId);

            // Assert
            await act.Should().ThrowAsync<FileNotFoundException>()
                .WithMessage("Profile image path is missing.");
        }
    }
}