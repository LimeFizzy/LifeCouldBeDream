using API.Models;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace API.Services
{
    public class UserScoreService : IUserScoreService
    {
        private readonly AppDbContext _dbContext;

        public UserScoreService(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IEnumerable<UserScore> GetLeaderboard()
        {
            try
            {
                return _dbContext.UserScores.AsNoTracking();
            }
            catch (Exception ex)
            {
                throw new IOException("An error occurred while retrieving the leaderboard data.", ex);
            }
        }

        public async Task SaveScoreAsync(UserScore userScore)
        {
            if (userScore == null)
            {
                throw new ArgumentNullException(nameof(userScore), "UserScore object cannot be null.");
            }

            try
            {
                _dbContext.UserScores.Add(userScore);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new DbUpdateException("An error occurred while saving the score to the database.", ex);
            }
        }

        public GameTypes? GetGameTypeFromString(string gameType)
        {
            return gameType switch
            {
                "longNumberMemory" => GameTypes.LONG_NUMBER,
                "sequenceMemory" => GameTypes.SEQUENCE,
                "chimpTest" => GameTypes.CHIMP,
                _ => null // Invalid game type
            };
        }
    }
}
