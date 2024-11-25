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
        [InlineData(
            "/api/auth/register", 
            "/api/auth/login", 
            $"/api/auth/is-admin/{username}",
            $"{{\"Username\":\"{username}\",\"Password\":\"{password}\"}}"
            )]
        public async Task Auth_EndpointsReturnSuccessAndCorrectContentType(string registerUrl, string loginUrl, string isAdminUrl, string body)
        {
            var client = _factory.CreateClient();

            var content = new StringContent(body ?? "{}", Encoding.UTF8, "application/json");
            var registerResponse = await client.PostAsync(registerUrl, content);
            var loginResponse = await client.PostAsync(loginUrl, content);
            var isAdminResponse = await client.GetAsync(isAdminUrl);
            
            registerResponse.EnsureSuccessStatusCode();
            loginResponse.EnsureSuccessStatusCode();
            isAdminResponse.EnsureSuccessStatusCode();

            Assert.Equal("application/json; charset=utf-8", registerResponse.Content.Headers.ContentType.ToString());
            Assert.Equal("application/json; charset=utf-8", loginResponse.Content.Headers.ContentType.ToString());
            Assert.Equal("application/json; charset=utf-8", isAdminResponse.Content.Headers.ContentType.ToString());
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
            var client = _factory.CreateClient();

            var content = new StringContent(body ?? "{}", Encoding.UTF8, "application/json");
            var submissionResponse = await client.PostAsync(submitUrl, content);
            var leaderboardResponse = await client.GetAsync(leaderboardUrl);

            submissionResponse.EnsureSuccessStatusCode();
            leaderboardResponse.EnsureSuccessStatusCode();
            
            Assert.Equal("application/json; charset=utf-8", submissionResponse.Content.Headers.ContentType.ToString());
            Assert.Equal("application/json; charset=utf-8", leaderboardResponse.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData($"/api/longnumber/generate-sequence/{level}")]
        [InlineData($"/api/sequence/generate-sequence/{level}")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            var client = _factory.CreateClient();
            HttpResponseMessage response;

            response = await client.GetAsync(url);
            
            response.EnsureSuccessStatusCode();

            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }
    }
}
