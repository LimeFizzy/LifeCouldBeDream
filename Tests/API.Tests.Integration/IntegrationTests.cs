using System;
using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Text;

namespace API.Tests.Integration
{
    public class BasicTests
        : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public BasicTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/auth/register", "POST", "{\"Username\":\"test10\",\"Password\":\"Test123.\",\"IsAdmin\":true}")]
        [InlineData("/api/auth/login", "POST", "{\"Username\":\"test10\",\"Password\":\"Test123.\",\"IsAdmin\":true}")]
        [InlineData("/api/longnumber/generate-sequence/1", "GET", null)]
        [InlineData("/api/sequence/generate-sequence/1", "GET", null)]
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

        [Theory]
        [InlineData("/api/UserScore/submit-score/longNumberMemory", "POST", "{\"username\":\"testUser\",\"gameType\":\"longNumberMemory\",\"guessedSequence\":[1,2,3],\"correctSequence\":[1,2,3],\"level\":1}")]
        [InlineData("/api/UserScore/submit-score/sequenceMemory", "POST", "{\"username\":\"testUser\",\"gameType\":\"sequenceMemory\",\"guessedSequence\":[4,5,6],\"correctSequence\":[4,5,6],\"level\":2}")]
        public async Task SubmitScore_EndpointsReturnSuccessAndCorrectContentType(string url, string method, string body)
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
                throw new NotSupportedException($"HTTP method {method} is not supported in this test.");
            }

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }


        [Theory]
        [InlineData("/api/UserScore/leaderboard/longNumberMemory", "GET", null)]
        [InlineData("/api/UserScore/leaderboard/sequenceMemory", "GET", null)]
        public async Task Leaderboard_EndpointsReturnSuccessAndCorrectContentType(string url, string method, string body)
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
