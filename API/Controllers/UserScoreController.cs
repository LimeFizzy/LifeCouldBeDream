using System;
using API.Models;
using System.Linq;
using API.Interfaces;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserScoreController(
        ILongNumberService longNumberService,
        ISequenceService sequenceService,
        IUserScoreService userScoreService,
        ILogger<UserScoreController> logger) : ControllerBase
    {
        private readonly ILongNumberService _longNumberService = longNumberService ?? throw new ArgumentNullException(nameof(longNumberService));
        private readonly ISequenceService _sequenceService = sequenceService ?? throw new ArgumentNullException(nameof(sequenceService));
        private readonly IUserScoreService _userScoreService = userScoreService ?? throw new ArgumentNullException(nameof(userScoreService));
        private readonly ILogger<UserScoreController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Thread-safe in-memory storage for user scores
        private static readonly ConcurrentDictionary<string, ConcurrentBag<UserScore>> _scores = new();

        [HttpGet("leaderboard/{gameType}")]
        public IActionResult GetLeaderboard(string gameType)
        {
            try
            {
                // Retrieve scores from memory or fallback to persistent storage
                var leaderboard = _scores.ContainsKey(gameType)
                    ? _scores[gameType].ToList()
                    : _userScoreService.GetLeaderboard()
                        .Where(score => score.GameType == gameType)
                        .ToList();

                if (!leaderboard.Any())
                {
                    _logger.LogWarning("No scores found for game type: {GameType}", gameType);
                    return NotFound(new { Message = $"No scores found for game type: {gameType}" });
                }

                // Sort by score (descending) and format dates
                var sortedLeaderboard = leaderboard
                    .OrderByDescending(us => us.Score)
                    .ToList();

                sortedLeaderboard.ForEach(us =>
                {
                    if (DateTime.TryParse(us.GameDate, out var parsedDate))
                    {
                        us.GameDate = parsedDate.ToString("yyyy/MM/dd");
                    }
                });

                return Ok(sortedLeaderboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the leaderboard for game type: {GameType}", gameType);
                return StatusCode(500, new { Message = "An error occurred while fetching the leaderboard." });
            }
        }

        [HttpPost("submit-score/{gameType}")]
        public async Task<IActionResult> SubmitScore([FromBody] ScoreSubmission submission, string gameType)
        {
            if (submission == null || string.IsNullOrWhiteSpace(submission.Username))
            {
                _logger.LogWarning("Invalid score submission data for game type: {GameType}", gameType);
                return BadRequest(new { Message = "Invalid score submission data." });
            }

            var parsedGameType = _userScoreService.GetGameTypeFromString(gameType);
            if (parsedGameType == null)
            {
                _logger.LogWarning("Invalid game type provided: {GameType}", gameType);
                return BadRequest(new { Message = $"Invalid game type: {gameType}" });
            }

            try
            {
                int score = parsedGameType switch
                {
                    GameTypes.LONG_NUMBER => _longNumberService.CalculateScore(submission.Level),
                    GameTypes.SEQUENCE => _sequenceService.CalculateScore(submission.Level),
                    GameTypes.CHIMP => throw new NotImplementedException("Chimp test game not implemented yet"),
                    _ => throw new ArgumentException($"Unhandled game type: {gameType}")
                };

                var userScore = new UserScore
                {
                    Username = submission.Username,
                    Score = score,
                    GameType = gameType,
                    GameDate = DateTime.Now.ToString()
                };

                // Add score to in-memory collection
                _scores.AddOrUpdate(
                    gameType,
                    new ConcurrentBag<UserScore> { userScore },
                    (key, existingBag) =>
                    {
                        existingBag.Add(userScore);
                        return existingBag;
                    });

                // Persist the score asynchronously
                await _userScoreService.SaveScoreAsync(userScore);

                return Ok(new { Message = "Score saved successfully", Score = score });
            }
            catch (NotImplementedException ex)
            {
                _logger.LogWarning(ex, "Feature not implemented for game type: {GameType}", gameType);
                return StatusCode(501, new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument provided for game type: {GameType}", gameType);
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while submitting the score for game type: {GameType}", gameType);
                return StatusCode(500, new { Message = "An error occurred while submitting the score." });
            }
        }
    }
}
