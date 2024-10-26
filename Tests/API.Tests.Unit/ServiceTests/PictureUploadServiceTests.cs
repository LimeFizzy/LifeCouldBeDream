namespace API.Tests.Unit.ServiceTests
{
    public class PictureUploadServiceTests
    {
        private readonly PictureUploadService _pictureUploadService;
        private readonly AppDbContext _dbContext;
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;

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

            // Initialize the service with the real DbContext
            _pictureUploadService = new PictureUploadService(_dbContext);
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
                ProfileImagePath = "uploads/image.jpg"
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Simulate the existence of the file
            File.WriteAllText(user.ProfileImagePath, "fake image content");

            // Act
            var result = await _pictureUploadService.GetProfileImageAsync(userId);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().BeEquivalentTo(File.ReadAllBytes(user.ProfileImagePath));
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
                .WithMessage("User or profile image not found.");
        }
    }
}
