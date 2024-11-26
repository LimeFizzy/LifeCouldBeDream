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
        public async Task<UserScore> GetScoreByIdAsync(int scoreId)
        {
            if (scoreId <= 0)
            {
                _logger.LogWarning("Invalid score ID provided: {ScoreId}.", scoreId);
                throw new ArgumentException("Score ID must be greater than zero.", nameof(scoreId));
            }

            try
            {
                var score = await _dbContext.UserScores.AsNoTracking().FirstOrDefaultAsync(us => us.Id == scoreId);
                if (score == null)
                {
                    _logger.LogWarning("Score not found for ID: {ScoreId}.", scoreId);
                }

                return score;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the score with ID: {ScoreId}.", scoreId);
                throw new IOException("An error occurred while retrieving the score.", ex);
            }
        }
        public async Task DeleteScoreAsync(UserScore score)
        {
            if (score == null)
            {
                _logger.LogWarning("Attempted to delete a null UserScore object.");
                throw new ArgumentNullException(nameof(score), "UserScore object cannot be null.");
            }

            try
            {
                _dbContext.UserScores.Remove(score);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Score deleted successfully for user {Username} with ID {ScoreId}.", score.Username, score.Id);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the score with ID {ScoreId} for user {Username}.", score.Id, score.Username);
                throw new DbUpdateException("An error occurred while deleting the score from the database.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while deleting the score with ID {ScoreId} for user {Username}.", score.Id, score.Username);
                throw;
            }
        }
    }
}
