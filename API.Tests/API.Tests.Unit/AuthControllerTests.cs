// File: API.Tests/UnitTests/AuthControllerTests.cs

using API.Controllers;
using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace API.Tests.UnitTests.Controllers
{
    public class AuthControllerTests : TestBase
    {
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            // Initialize the controller using the mocked context and service
            _controller = new AuthController(MockContext.Object, MockAuthService.Object);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenUsernameExists()
        {
            // Arrange
            var userDto = new UserDto { Username = "testUser", Password = "password123" };
            MockContext.Setup(c => c.Users.AnyAsync(It.IsAny<Expression<Func<User, bool>>>()))
                        .ReturnsAsync(true); // Simulate that the user exists

            // Act
            var result = await _controller.Register(userDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Username is already taken.", badRequestResult.Value);
        }

        [Fact]
        public async Task Register_ReturnsCreatedAtAction_WhenUserIsRegisteredSuccessfully()
        {
            // Arrange
            var userDto = new UserDto { Username = "newUser", Password = "password123" };
            MockContext.Setup(c => c.Users.AnyAsync(It.IsAny<Expression<Func<User, bool>>>()))
                        .ReturnsAsync(false); // User doesn't exist
            MockAuthService.Setup(a => a.HashPassword(userDto.Password)).Returns("hashedPassword");

            // Act
            var result = await _controller.Register(userDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(_controller.GetUserById), createdAtActionResult.ActionName);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenUserDoesNotExist()
        {
            // Arrange
            var loginDto = new LoginDto { Username = "nonExistentUser", Password = "password123" };
            MockContext.Setup(c => c.Users.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
                        .ReturnsAsync((User)null); // Simulate that the user does not exist

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("User doesn't exist", unauthorizedResult.Value);
        }

        [Fact]
        public async Task IsAdmin_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            string username = "nonExistentUser";
            MockContext.Setup(c => c.Users.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
                        .ReturnsAsync((User)null); // Simulate that the user does not exist

            // Act
            var result = await _controller.IsAdmin(username);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found.", notFoundResult.Value);
        }

    }
}
