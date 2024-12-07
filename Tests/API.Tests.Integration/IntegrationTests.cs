using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using API.DTOs;
using API.Models;
using System;
using System.Collections.Generic;

namespace API.Tests.Integration
{
    // Wrapwer class for deserialization of UserScore objects
    public class SubmitScoreResponse
    {
        public string Message { get; set; }
        public UserScore Score { get; set; }
    }

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

            Assert.Equal(username, registeredValue.Username);
            Assert.Equal(username, loginValue.Username);
            Assert.Equal(isAdmin, isAdminValue.IsAdmin);
            Assert.Equal(username, deleteValue.Username);
        }

        [Theory]
        [InlineData(
            "longNumberMemory",
            "/api/UserScore/submit-score/longNumberMemory", 
            "/api/UserScore/leaderboard/longNumberMemory", 
            $"{{\"username\":\"{username}\",\"gameType\":\"longNumberMemory\",\"level\":{level}}}"
            )]
        [InlineData(
            "sequenceMemory",
            "/api/UserScore/submit-score/sequenceMemory", 
            "/api/UserScore/leaderboard/sequenceMemory", 
            $"{{\"username\":\"{username}\",\"gameType\":\"sequenceMemory\",\"level\":{level}}}"
            )]
        [InlineData(
            "chimpTest",
            "/api/UserScore/submit-score/chimpTest",
            "/api/UserScore/leaderboard/chimpTest",
            $"{{\"username\":\"{username}\",\"gameType\":\"chimpTest\",\"level\":{level}}}"
        )]
        public async Task SubmitScore_EndpointsReturnSuccessAndCorrectContentType(string gameType, string submitUrl, string leaderboardUrl, string body)
        {
            var client = _factory.CreateClient();

            var content = new StringContent(body ?? "{}", Encoding.UTF8, "application/json");

            var submissionResponse = await client.PostAsync(submitUrl, content);
            var submissionRead = await submissionResponse.Content.ReadAsStringAsync();
            var submissionValue = JsonConvert.DeserializeObject<SubmitScoreResponse>(submissionRead);

            var leaderboardResponse = await client.GetAsync(leaderboardUrl);
            var leaderboardRead = await leaderboardResponse.Content.ReadAsStringAsync();
            var leaderboardValue = JsonConvert.DeserializeObject<List<UserScore>>(leaderboardRead);

            var deleteUrl = $"/api/UserScore/delete-leaderboard/{submissionValue.Score.Id}";
            var deleteResponse = await client.DeleteAsync(deleteUrl);

          

            submissionResponse.EnsureSuccessStatusCode();
            leaderboardResponse.EnsureSuccessStatusCode();
            deleteResponse.EnsureSuccessStatusCode();

            Assert.NotNull(submissionValue);
            Assert.True(submissionValue.Score.Id > 0);
            Assert.Contains(leaderboardValue, score => score.Id == submissionValue.Score.Id);
            
            int parsedLevel = int.Parse(level);
            
            switch (gameType) {
                case "longNumberMemory":
                    Assert.Equal(int.Parse(level) - 1, submissionValue.Score.Score);
                    break;
                case "sequenceMemory":
                    parsedLevel = int.Parse(level);
                    parsedLevel--;
                    Assert.Equal(parsedLevel <= 2 ? parsedLevel : parsedLevel * (parsedLevel - 1) / 2, submissionValue.Score.Score);
                    break;
                case "chimpTest":
                    parsedLevel--;
                    int expectedChimpScore = (parsedLevel * parsedLevel + 5 * parsedLevel) / 2;
                    Assert.Equal(expectedChimpScore, submissionValue.Score.Score);
                    break;
                default:
                    throw new ArgumentException("Invalid game type");
            }

            leaderboardResponse = await client.GetAsync(leaderboardUrl);
            leaderboardResponse.EnsureSuccessStatusCode();
            leaderboardRead = await leaderboardResponse.Content.ReadAsStringAsync();
            leaderboardValue = JsonConvert.DeserializeObject<List<UserScore>>(leaderboardRead);

            Assert.DoesNotContain(leaderboardValue, score => score.Id == submissionValue.Score.Id);
        }
    }
}
