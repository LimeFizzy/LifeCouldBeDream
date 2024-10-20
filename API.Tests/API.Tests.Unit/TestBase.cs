using API.Data;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq.Expressions;
using Xunit;

namespace API.Tests.UnitTests
{
    public abstract class TestBase : IDisposable
    {
        protected readonly Mock<AppDbContext> MockContext;
        protected readonly Mock<AuthService> MockAuthService;

        public TestBase()
        {
            MockContext = new Mock<AppDbContext>();
            MockAuthService = new Mock<AuthService>();
        }

        protected void SetupMockUser(string username, string passwordHash, bool isAdmin = false)
        {
            var user = new User { Username = username, PasswordHash = passwordHash, IsAdmin = isAdmin };

            MockContext.Setup(c => c.Users.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
                        .ReturnsAsync(user);
        }

        public void Dispose()
        {
            MockContext?.Dispose();
        }
    }
}
