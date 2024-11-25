using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Text;

namespace API.Tests.Integration {
    public class BasicTests 
        : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public BasicTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }



        [Theory]
        [InlineData("/api/auth/register", "POST", "{\"Username\":\"test\",\"Password\":\"test123\",\"IsAdmin\":1}")]
        [InlineData("/api/auth/login", "POST", "{\"Username\":\"test\",\"Password\":\"test123\"}")]
        //[InlineData("/api/auth/is-admin/username")]
        //[InlineData("/api/auth/1")]
        [InlineData("/api/longnumber/generate-sequence/1", "GET", null)]
        [InlineData("/api/sequence/generate-sequence/1", "GET", null)]
        //[InlineData("/api/pictureupload/1/profile-image")]
        //[InlineData("/api/pictureupload/upload-profile-image/1")]
        [InlineData("/api/userscore/submit-score/longNumberMemory", "POST", "{\"Username\":\"testUser\",\"Level\":1}")]
        [InlineData("/api/userscore/leaderboard/longNumberMemory", "GET", null)]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url, string method, string body)
        {
            // Arrange
            var client = _factory.CreateClient();
            HttpResponseMessage response;

            // Act
            if (method == "POST")
            {
                var content = new StringContent(body ?? "{}", Encoding.UTF8, "application/json");
                response = await client.PostAsync(url, content);
            }
            else
            {
                response = await client.GetAsync(url);
            }

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8", 
                response.Content.Headers.ContentType.ToString());
        }
    }
}