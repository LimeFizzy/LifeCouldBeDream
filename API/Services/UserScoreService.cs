using API.Data;
using System.IO;
using API.Models;
using API.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace API.Services
{
    public class UserScoreService : IUserScoreService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<UserScoreService> _logger;

        public UserScoreService(AppDbContext dbContext, ILogger<UserScoreService> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEnumerable<UserScore> GetLeaderboard()
        {
            try
            {
                var leaderboard = _dbContext.UserScores.AsNoTracking();
                return leaderboard;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the leaderboard data.");
                throw new IOException("An error occurred while retrieving the leaderboard data.", ex);
            }
        }

        public async Task SaveScoreAsync(UserScore userScore)
        {
            if (userScore == null)
            {
                _logger.LogWarning("Attempted to save a null UserScore object.");
                throw new ArgumentNullException(nameof(userScore), "UserScore object cannot be null.");
            }

            try
            {
                _dbContext.UserScores.Add(userScore);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while saving the score to the database for user {Username}.", userScore.Username);
                throw new DbUpdateException("An error occurred while saving the score to the database.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while saving the score for user {Username}.", userScore.Username);
                throw;
            }
        }

        public GameTypes? GetGameTypeFromString(string gameType)
        {
            var result = gameType switch
            {
                "longNumberMemory" => GameTypes.LONG_NUMBER,
                "sequenceMemory" => GameTypes.SEQUENCE,
                "chimpTest" => GameTypes.CHIMP,
                _ => (GameTypes?)null
            };

            if (result == null)
            {
                _logger.LogWarning("Invalid game type string provided: {GameTypeString}.", gameType);
            }

            return result;
        }
    }
}
