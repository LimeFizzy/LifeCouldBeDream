using API.Models;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Interfaces;

namespace API.Services
{
    public class UserScoreService(AppDbContext dbContext) : IUserScoreService
    {
        private readonly AppDbContext _dbContext = dbContext;

        public IEnumerable<UserScore> GetLeaderboard()
        {
            return _dbContext.UserScores.AsNoTracking();
        }

        public async Task SaveScoreAsync(UserScore userScore)
        {
            _dbContext.UserScores.Add(userScore);
            await _dbContext.SaveChangesAsync();
        }

        public GameTypes? GetGameTypeFromString(string gameType)
        {
            return gameType switch
            {
                "longNumberMemory" => (GameTypes?)GameTypes.LONG_NUMBER,
                "sequenceMemory" => (GameTypes?)GameTypes.SEQUENCE,
                "chimpTest" => (GameTypes?)GameTypes.CHIMP,
                _ => null,// Invalid game type
            };
        }
    }
}
