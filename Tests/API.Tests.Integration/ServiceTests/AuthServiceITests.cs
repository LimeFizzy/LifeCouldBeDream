using System;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using API.Services;
using API.Exceptions;

namespace API.Tests.Integration.ServiceTests
{
    public class AuthServiceIntegrationTests
    {
        private readonly AuthService _authService;

        public AuthServiceIntegrationTests()
        {
            // Difference from unit tests - use a NullLogger for testing (no actual log output)
            var logger = NullLogger<AuthService>.Instance;
            _authService = new AuthService(logger);
        }

        [Fact]
        public void HashPassword_ShouldReturnHashedPassword_ForValidInput()
        {
            var password = "StrongPassword123!";

            var hashedPassword = _authService.HashPassword(password);

            hashedPassword.Should().NotBeNullOrEmpty();
            hashedPassword.Should().NotBe(password);
        }

        [Fact]
        public void HashPassword_ShouldThrowArgumentNullException_ForNullPassword()
        {
            string password = null;

            Action act = () => _authService.HashPassword(password);

            act.Should().Throw<ArgumentNullException>().WithMessage("*Password cannot be null or empty*");
        }

        [Fact]
        public void VerifyPassword_ShouldReturnTrue_ForMatchingPasswords()
        {
            var password = "ValidPassword123!";
            var hashedPassword = _authService.HashPassword(password);

            var result = _authService.VerifyPassword(password, hashedPassword);

            result.Should().BeTrue();
        }

        [Fact]
        public void VerifyPassword_ShouldReturnFalse_ForMismatchedPasswords()
        {
            var password = "ValidPassword123!";
            var hashedPassword = _authService.HashPassword(password);
            var wrongPassword = "InvalidPassword456!";

            var result = _authService.VerifyPassword(wrongPassword, hashedPassword);

            result.Should().BeFalse();
        }

        [Fact]
        public void VerifyPassword_ShouldThrowArgumentNullException_ForNullPassword()
        {
            string password = null;
            string storedHash = "dummyHash";

            Action act = () => _authService.VerifyPassword(password, storedHash);

            act.Should().Throw<ArgumentNullException>().WithMessage("*Password cannot be null or empty*");
        }

        [Fact]
        public void ValidatePasswordStrength_ShouldNotThrow_ForStrongPassword()
        {
            var password = "StrongP@ssword123";

            Action act = () => _authService.ValidatePasswordStrength(password);

            act.Should().NotThrow();
        }

        [Theory]
        [InlineData("")]
        [InlineData("1234")]
        [InlineData("password")]
        [InlineData("PASSWORD123")]
        [InlineData("Password")]
        [InlineData("Password1")]
        public void ValidatePasswordStrength_ShouldThrowWeakPasswordException_ForInvalidPasswords(string password)
        {
            Action act = () => _authService.ValidatePasswordStrength(password);

            act.Should().Throw<WeakPasswordException>();
        }
    }
}
