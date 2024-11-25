using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Text;

namespace API.Tests.Integration
{
    public class IntegrationTests
        : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private const string username = "test10";
        private const string password = "Test123.";
        private const string level = "2";

        public IntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/auth/register", "POST", $"{{\"Username\":\"{username}\",\"Password\":\"{password}\"}}")]
        [InlineData("/api/auth/login", "POST", $"{{\"Username\":\"{username}\",\"Password\":\"{password}\"}}")]
        [InlineData($"/api/auth/is-admin/{username}", "GET", null)]
        public async Task Auth_EndpointsReturnSuccessAndCorrectContentType(string url, string method, string body)
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

            // Get and log the response body
            var responseBody = await response.Content.ReadAsStringAsync();
            
            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            //Assert.False(true, responseBody);     // for testing purpose only!
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData(
            "/api/UserScore/submit-score/longNumberMemory", 
            "/api/UserScore/leaderboard/longNumberMemory", 
            $"{{\"username\":\"{username}\",\"gameType\":\"longNumberMemory\",\"level\":{level}}}"
            )]
        [InlineData(
            "/api/UserScore/submit-score/sequenceMemory", 
            "/api/UserScore/leaderboard/sequenceMemory", 
            $"{{\"username\":\"{username}\",\"gameType\":\"sequenceMemory\",\"level\":{level}}}"
            )]
        public async Task SubmitScore_EndpointsReturnSuccessAndCorrectContentType(string submitUrl, string leaderboardUrl, string body)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var content = new StringContent(body ?? "{}", Encoding.UTF8, "application/json");
            var submissionResponse = await client.PostAsync(submitUrl, content);
            var leaderboardResponse = await client.GetAsync(leaderboardUrl);

            // Assert
            submissionResponse.EnsureSuccessStatusCode(); // Status Code 200-299
            leaderboardResponse.EnsureSuccessStatusCode();
            
            Assert.Equal("application/json; charset=utf-8", submissionResponse.Content.Headers.ContentType.ToString());
            Assert.Equal("application/json; charset=utf-8", leaderboardResponse.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData($"/api/longnumber/generate-sequence/{level}")]
        [InlineData($"/api/sequence/generate-sequence/{level}")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            HttpResponseMessage response;

            // Act
                response = await client.GetAsync(url);

            // Get and log the response body
            var responseBody = await response.Content.ReadAsStringAsync();
            
            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            //Assert.False(true, responseBody);     // for testing purpose only!
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
}
