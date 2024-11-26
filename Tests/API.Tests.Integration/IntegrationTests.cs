using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using API.DTOs;

namespace API.Tests.Integration
{
    public class IntegrationTests
        : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private const string username = "IntegrationTestUser";
        private const string password = "ITPass123~!";

        private const bool isAdmin = false;
        private const string level = "2";

        public IntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData(
            "/api/auth/register", 
            "/api/auth/login", 
            $"/api/auth/delete/{username}",
            $"/api/auth/is-admin/{username}",
            $"{{\"Username\":\"{username}\",\"Password\":\"{password}\"}}"
            )]
        public async Task Auth_EndpointsReturnSuccessAndCorrectContentType(string registerUrl, string loginUrl, string deleteUrl, string isAdminUrl, string body)
        {
            var client = _factory.CreateClient();

            var content = new StringContent(body ?? "{}", Encoding.UTF8, "application/json");
            var registerResponse = await client.PostAsync(registerUrl, content);
            var registerRead = await registerResponse.Content.ReadAsStringAsync();
            var registeredValue = JsonConvert.DeserializeObject<UserDto>(registerRead);
            
            var loginResponse = await client.PostAsync(loginUrl, content);
            var loginRead = await loginResponse.Content.ReadAsStringAsync();
            var loginValue = JsonConvert.DeserializeObject<UserDto>(loginRead);

            var isAdminResponse = await client.GetAsync(isAdminUrl);
            var isAdminRead = await isAdminResponse.Content.ReadAsStringAsync();
            var isAdminValue = JsonConvert.DeserializeObject<UserDto>(isAdminRead);

            var deleteResponse = await client.DeleteAsync(deleteUrl);
            var deleteRead = await deleteResponse.Content.ReadAsStringAsync();
            var deleteValue = JsonConvert.DeserializeObject<UserDto>(deleteRead);

            
            registerResponse.EnsureSuccessStatusCode();
            loginResponse.EnsureSuccessStatusCode();
            isAdminResponse.EnsureSuccessStatusCode();
            deleteResponse.EnsureSuccessStatusCode();

            Assert.Equal("application/json; charset=utf-8", registerResponse.Content.Headers.ContentType.ToString());
            Assert.Equal(username, registeredValue.Username);
            Assert.Equal("application/json; charset=utf-8", loginResponse.Content.Headers.ContentType.ToString());
            Assert.Equal(username, loginValue.Username);
            Assert.Equal("application/json; charset=utf-8", isAdminResponse.Content.Headers.ContentType.ToString());
            Assert.Equal(isAdmin, isAdminValue.IsAdmin);
            Assert.Equal("application/json; charset=utf-8", deleteResponse.Content.Headers.ContentType.ToString());
            Assert.Equal(username, deleteValue.Username);
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


    }
}
