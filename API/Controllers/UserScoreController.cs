using API.Models;
using API.Interfaces;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserScoreController : ControllerBase
    {
        private readonly IUserScoreService _userScoreService;
        private readonly IUnifiedGamesService<int> _uniServInt;
        private readonly IUnifiedGamesService<Square> _uniServSquare;
        private readonly IUnifiedGamesService<SquareChimp> _uniServSquareChimp;
        private readonly ILogger<UserScoreController> _logger;
        private static readonly ConcurrentDictionary<string, ConcurrentBag<UserScore>> _scores = new();
        private static bool _isInitialized = false;
        private static readonly object _initializationLock = new();

        public UserScoreController(
            IUserScoreService userScoreService,
            IUnifiedGamesService<int> uniServInt,
            IUnifiedGamesService<Square> uniServSquare,
            IUnifiedGamesService<SquareChimp> uniServSquareChimp,
            ILogger<UserScoreController> logger)
        {
            _userScoreService = userScoreService ?? throw new ArgumentNullException(nameof(userScoreService));
            _uniServInt = uniServInt ?? throw new ArgumentNullException(nameof(uniServInt));
            _uniServSquare = uniServSquare ?? throw new ArgumentNullException(nameof(uniServSquare));
            _uniServSquareChimp = uniServSquareChimp ?? throw new ArgumentNullException(nameof(uniServSquareChimp));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            InitializeScores();
        }

        private void InitializeScores()
        {
            lock (_initializationLock)
            {
                if (_isInitialized) return;

                try
                {
                    var allScores = _userScoreService.GetLeaderboard();

                    foreach (var scoreGroup in allScores.GroupBy(score => score.GameType))
                    {
                        var scoreBag = new ConcurrentBag<UserScore>(scoreGroup);
                        _scores.TryAdd(scoreGroup.Key, scoreBag);
                    }

                    _isInitialized = true;
                    _logger.LogInformation("In-memory scores initialized from database.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to initialize in-memory scores from database.");
                }
            }
        }

        [HttpGet("leaderboard/{gameType}")]
        public IActionResult GetLeaderboard(string gameType)
        {
            try
            {
                var leaderboard = _scores.TryGetValue(gameType, out var value)
                    ? value.ToList()
                    : _userScoreService.GetLeaderboard()
                        .Where(score => score.GameType == gameType)
                        .ToList();

                if (!leaderboard.Any())
                {
                    _logger.LogWarning("No scores found for game type: {GameType}", gameType);
                    return NotFound(new { Message = $"No scores found for game type: {gameType}" });
                }

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
                    GameTypes.LONG_NUMBER => _uniServInt.CalculateScore(submission.Level),
                    GameTypes.SEQUENCE => _uniServSquare.CalculateScore(submission.Level),
                    GameTypes.CHIMP => _uniServSquareChimp.CalculateScore(submission.Level),
                    _ => throw new ArgumentException($"Unhandled game type: {gameType}")
                };

                var userScore = new UserScore
                {
                    Username = submission.Username,
                    Score = score,
                    GameType = gameType,
                    GameDate = DateTime.Now.ToString()
                };

                var scoreBag = _scores.GetOrAdd(gameType, new ConcurrentBag<UserScore>());
                scoreBag.Add(userScore);

                await _userScoreService.SaveScoreAsync(userScore);

                return Ok(new { Message = "Score saved successfully", Score = userScore });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while submitting the score for game type: {GameType}", gameType);
                return StatusCode(500, new { Message = "An error occurred while submitting the score." });
            }
        }

        [HttpDelete("delete-leaderboard/{scoreId}")]
        public async Task<IActionResult> DeleteScore(int scoreId)
        {
            if (scoreId <= 0)
            {
                _logger.LogWarning("Invalid score ID provided for deletion: {ScoreId}", scoreId);
                return BadRequest(new { Message = "Invalid score ID." });
            }

            try
            {
                bool inMemoryDeleted = false;
                foreach (var gameType in _scores.Keys)
                {
                    if (_scores.TryGetValue(gameType, out var scoreBag))
                    {
                        var scoreToDelete = scoreBag.FirstOrDefault(score => score.Id == scoreId);
                        if (scoreToDelete != null)
                        {
                            var newScoreBag = new ConcurrentBag<UserScore>(scoreBag.Except(new[] { scoreToDelete }));
                            _scores[gameType] = newScoreBag;
                            inMemoryDeleted = true;
                            _logger.LogInformation("Score deleted from in-memory collection. Score ID: {ScoreId}", scoreId);
                            break;
                        }
                    }
                }

                if (!inMemoryDeleted)
                {
                    _logger.LogWarning("Score not found in in-memory storage for ID: {ScoreId}", scoreId);
                }

                var scoreFromDb = await _userScoreService.GetScoreByIdAsync(scoreId);
                if (scoreFromDb == null)
                {
                    _logger.LogWarning("Score not found in database for ID: {ScoreId}", scoreId);
                    return NotFound(new { Message = "Score not found." });
                }

                await _userScoreService.DeleteScoreAsync(scoreFromDb);

                return Ok(new { Message = $"Score with ID '{scoreId}' has been deleted." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting score with ID: {ScoreId}", scoreId);
                return StatusCode(500, new { Message = "An error occurred while deleting the score." });
            }
        }
    }
}
