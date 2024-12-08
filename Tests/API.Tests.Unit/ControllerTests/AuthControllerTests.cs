using Newtonsoft.Json;

namespace API.Tests.Unit.ControllerTests
{
    public class AuthControllerTests : IDisposable
    {
        private readonly AppDbContext _dbContext;
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            // Setup in-memory database with a unique name for isolation
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new AppDbContext(options);
            _mockAuthService = new Mock<IAuthService>();
            _mockLogger = new Mock<ILogger<AuthController>>();

            _controller = new AuthController(_dbContext, _mockAuthService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenLoginIsSuccessful()
        {
            // Arrange
            var loginDto = new LoginDto { Username = "authTestUser", Password = "StrongPass123!" };
            var user = new User { Username = "authTestUser", PasswordHash = "hashedPassword" };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync(); // This generates the UserId

            _mockAuthService.Setup(auth => auth.VerifyPassword(loginDto.Password, user.PasswordHash)).Returns(true);

            // Act
            var loginResult = await _controller.Login(loginDto);

            // Assert
            if (loginResult.Result is OkObjectResult loginOkResult)
            {
                // Serialize the anonymous object to JSON
                var jsonResult = JsonConvert.SerializeObject(loginOkResult.Value);

                // Deserialize the JSON to a dynamic object
                dynamic actualValue = JsonConvert.DeserializeObject<dynamic>(jsonResult);

                int returnedUserId = actualValue.userId;
                string returnedUsername = actualValue.username;

                // Retrieve the user from the database to get the generated UserId
                var userInDb = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
                Assert.NotNull(userInDb);

                // Assert that the returned values match the expected values
                Assert.Equal(userInDb.UserId, returnedUserId);
                Assert.Equal(userInDb.Username, returnedUsername);
            }
            else
            {
                Assert.True(false, $"Expected OkObjectResult, but got {loginResult.Result.GetType().Name}.");
            }
        }

        [Fact]
        public async Task Register_ReturnsCreated_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var userDto = new UserDto { Username = "authTestnewUser", Password = "StrongPass123!" };

            // Ensure the password validation passes and hashing returns a known hash
            _mockAuthService.Setup(auth => auth.ValidatePasswordStrength(It.IsAny<string>()));
            _mockAuthService.Setup(auth => auth.HashPassword(userDto.Password)).Returns("hashedPassword");

            // Act
            var result = await _controller.Register(userDto);

            // Assert
            if (result.Result is CreatedAtActionResult createdAtResult)
            {
                var createdUser = createdAtResult.Value as User;
                Assert.NotNull(createdUser);

                // Retrieve the user from the database to get the generated UserId
                var userInDb = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == userDto.Username);
                Assert.NotNull(userInDb);

                Assert.Equal(userInDb.UserId, createdUser.UserId);
                Assert.Equal(userDto.Username, createdUser.Username);
                Assert.Equal("hashedPassword", createdUser.PasswordHash);
            }
            else
            {
                Assert.True(false, $"Expected CreatedAtActionResult, but got {result.Result.GetType().Name}.");
            }
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenUserAlreadyExists()
        {
            // Arrange
            var userDto = new UserDto { Username = "authTestexistingUser", Password = "StrongPass123!" };
            var existingUser = new User { Username = "authTestexistingUser", PasswordHash = "hashedPassword" };

            await _dbContext.Users.AddAsync(existingUser);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.Register(userDto);

            // Assert
            if (result.Result is BadRequestObjectResult badRequestResult)
            {
                var jsonResult = JsonConvert.SerializeObject(badRequestResult.Value);
                dynamic actualValue = JsonConvert.DeserializeObject<dynamic>(jsonResult);
                string actualMessage = actualValue.Message;

                Assert.Equal("Username is already taken.", actualMessage);
            }
            else
            {
                Assert.True(false, $"Expected BadRequestObjectResult, but got {result.Result.GetType().Name}.");
            }
        }

        [Fact]
        public async Task Login_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var loginDto = new LoginDto { Username = "nonExistentUser", Password = "StrongPass123!" };

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            if (result.Result is NotFoundObjectResult notFoundResult)
            {
                var jsonResult = JsonConvert.SerializeObject(notFoundResult.Value);
                dynamic actualValue = JsonConvert.DeserializeObject<dynamic>(jsonResult);
                string actualMessage = actualValue.Message;

                Assert.Equal("User does not exist.", actualMessage);
            }
            else
            {
                Assert.True(false, $"Expected NotFoundObjectResult, but got {result.Result.GetType().Name}.");
            }
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenPasswordIsInvalid()
        {
            // Arrange
            var loginDto = new LoginDto { Username = "testUser", Password = "WrongPass123!" };
            var user = new User { Username = "testUser", PasswordHash = "hashedPassword" };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            _mockAuthService.Setup(auth => auth.VerifyPassword(loginDto.Password, user.PasswordHash)).Returns(false);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            if (result.Result is UnauthorizedObjectResult unauthorizedResult)
            {
                var jsonResult = JsonConvert.SerializeObject(unauthorizedResult.Value);
                dynamic actualValue = JsonConvert.DeserializeObject<dynamic>(jsonResult);
                string actualMessage = actualValue.Message;

                Assert.Equal("Invalid username or password.", actualMessage);
            }
            else
            {
                Assert.True(false, $"Expected UnauthorizedObjectResult, but got {result.Result.GetType().Name}.");
            }
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenUserIsDeleted()
        {
            // Arrange
            var user = new User { Username = "testUser", PasswordHash = "hashedPassword" };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.Delete(user.Username);

            // Assert
            if (result.Result is OkObjectResult okResult)
            {
                var jsonResult = JsonConvert.SerializeObject(okResult.Value);
                dynamic actualValue = JsonConvert.DeserializeObject<dynamic>(jsonResult);
                string actualMessage = actualValue.Message;

                Assert.Equal("User deleted successfully.", actualMessage);

                var userInDb = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
                Assert.Null(userInDb);
            }
            else
            {
                Assert.True(false, $"Expected OkObjectResult, but got {result.Result.GetType().Name}.");
            }
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_WhenUserDoesNotExist()
        {
            // Arrange
            var username = "nonExistentUser";

            // Act
            var result = await _controller.Delete(username);

            // Assert
            if (result.Result is BadRequestObjectResult badRequestResult)
            {
                var jsonResult = JsonConvert.SerializeObject(badRequestResult.Value);
                dynamic actualValue = JsonConvert.DeserializeObject<dynamic>(jsonResult);
                string actualMessage = actualValue.Message;

                Assert.Equal("Username does not exist", actualMessage);
            }
            else
            {
                Assert.True(false, $"Expected BadRequestObjectResult, but got {result.Result.GetType().Name}.");
            }
        }

        [Fact]
        public async Task IsAdmin_ReturnsOk_WhenUserIsAdmin()
        {
            // Arrange
            var user = new User { Username = "adminUser", PasswordHash = "hashedPassword", IsAdmin = true };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.IsAdmin(user.Username);

            // Assert
            if (result is OkObjectResult okResult)
            {
                var jsonResult = JsonConvert.SerializeObject(okResult.Value);
                dynamic actualValue = JsonConvert.DeserializeObject<dynamic>(jsonResult);
                bool isAdmin = actualValue.IsAdmin;

                Assert.True(isAdmin);
            }
            else
            {
                Assert.True(false, $"Expected OkObjectResult, but got {result.GetType().Name}.");
            }
        }

        [Fact]
        public async Task IsAdmin_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var username = "nonExistentUser";

            // Act
            var result = await _controller.IsAdmin(username);

            // Assert
            if (result is NotFoundObjectResult notFoundResult)
            {
                var jsonResult = JsonConvert.SerializeObject(notFoundResult.Value);
                dynamic actualValue = JsonConvert.DeserializeObject<dynamic>(jsonResult);
                string actualMessage = actualValue.Message;

                Assert.Equal("User not found.", actualMessage);
            }
            else
            {
                Assert.True(false, $"Expected NotFoundObjectResult, but got {result.GetType().Name}.");
            }
        }


        [Fact]
        public async Task ChangePassword_ReturnsOk_WhenPasswordIsChangedSuccessfully()
        {
            // Arrange
            var user = new User { Username = "testUser", PasswordHash = "hashedOldPassword" };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var changePasswordDto = new ChangePasswordDto
            {
                Username = user.Username,
                OldPassword = "oldPassword123!",
                NewPassword = "newStrongPassword123!"
            };

            _mockAuthService.Setup(auth => auth.VerifyPassword(changePasswordDto.OldPassword, user.PasswordHash)).Returns(true);
            _mockAuthService.Setup(auth => auth.HashPassword(changePasswordDto.NewPassword)).Returns("hashedNewPassword");

            // Act
            var result = await _controller.ChangePassword(changePasswordDto);

            // Assert
            if (result is OkObjectResult okResult)
            {
                var jsonResult = JsonConvert.SerializeObject(okResult.Value);
                dynamic actualValue = JsonConvert.DeserializeObject<dynamic>(jsonResult);
                string actualMessage = actualValue.Message;

                Assert.Equal("Password changed successfully.", actualMessage);

                var userInDb = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
                Assert.NotNull(userInDb);
                Assert.Equal("hashedNewPassword", userInDb.PasswordHash);
            }
            else
            {
                Assert.True(false, $"Expected OkObjectResult, but got {result.GetType().Name}.");
            }
        }

        [Fact]
        public async Task ChangePassword_ReturnsUnauthorized_WhenOldPasswordIsIncorrect()
        {
            // Arrange
            var user = new User { Username = "testUser", PasswordHash = "hashedOldPassword" };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var changePasswordDto = new ChangePasswordDto
            {
                Username = user.Username,
                OldPassword = "wrongOldPassword123!",
                NewPassword = "newStrongPassword123!"
            };

            _mockAuthService.Setup(auth => auth.VerifyPassword(changePasswordDto.OldPassword, user.PasswordHash)).Returns(false);

            // Act
            var result = await _controller.ChangePassword(changePasswordDto);

            // Assert
            if (result is UnauthorizedObjectResult unauthorizedResult)
            {
                var jsonResult = JsonConvert.SerializeObject(unauthorizedResult.Value);
                dynamic actualValue = JsonConvert.DeserializeObject<dynamic>(jsonResult);
                string actualMessage = actualValue.Message;

                Assert.Equal("Invalid old password.", actualMessage);
            }
            else
            {
                Assert.True(false, $"Expected UnauthorizedObjectResult, but got {result.GetType().Name}.");
            }
        }


        [Fact]
        public async Task ChangePassword_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var changePasswordDto = new ChangePasswordDto
            {
                Username = "nonExistentUser",
                OldPassword = "oldPassword123!",
                NewPassword = "newStrongPassword123!"
            };

            // Act
            var result = await _controller.ChangePassword(changePasswordDto);

            // Assert
            if (result is NotFoundObjectResult notFoundResult)
            {
                var jsonResult = JsonConvert.SerializeObject(notFoundResult.Value);
                dynamic actualValue = JsonConvert.DeserializeObject<dynamic>(jsonResult);
                string actualMessage = actualValue.Message;

                Assert.Equal("User not found.", actualMessage);
            }
            else
            {
                Assert.True(false, $"Expected NotFoundObjectResult, but got {result.GetType().Name}.");
            }
        }

        public void Dispose()
        {
            // Clean up in-memory database after each test
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}