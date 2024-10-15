using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserScoreController : ControllerBase
    {
        private readonly LongNumberService _longNumberService;
        private readonly UserScoreService _userScoreService;

        public UserScoreController(LongNumberService longNumberService, UserScoreService userScoreService)
        {
            _longNumberService = longNumberService;
            _userScoreService = userScoreService;
        }

        [HttpGet("leaderboard/{gameType}")]
        public IActionResult GetLeaderboard(string gameType)
        {
            try
            {
                var leaderboard = _userScoreService.GetLeaderboard()
                                    .Where(score => score.GameType == gameType)
                                    .ToList();

                // Create a list of UserScoreComparer objects and sort them
                var sortedLeaderboard = leaderboard
                                        .Select(score => new UserScoreComparer(score))
                                        .OrderBy(scoreComparer => scoreComparer) // This now uses IComparable<UserScoreComparer>
                                        .ToList();

                var leaderboardWithTime = new List<object>();

                // Use foreach to process leaderboard entries
                foreach (var scoreComparer in sortedLeaderboard)
                {
                    var score = scoreComparer.UserScore; // Access the original UserScore object

                    leaderboardWithTime.Add(new
                    {
                        score.Username,
                        score.Score,
                        GameDate = score.GameDate.ToString("o") // Format time and date. Hehe ISO 8601 otherwise, not happy
                    });
                }

                return Ok(leaderboardWithTime);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching the leaderboard: " + ex);
            }
        }

        [HttpPost("submit-score/{gameType}")]
        public async Task<IActionResult> SubmitScore([FromBody] ScoreSubmission submission, string gameType)
        {
            var parsedGameType = _userScoreService.GetGameTypeFromString(gameType);
            if (parsedGameType == null)
            {
                return BadRequest(new { Message = $"Invalid game type: {gameType}" });
            }

            int score;

            switch (parsedGameType)
            {
                case GameTypes.LONG_NUMBER:
                    score = _longNumberService.CalculateScore(submission.Level);
                    break;

                case GameTypes.SEQUENCE:
                    // Not Implemented
                    return StatusCode(501, new { Message = "Sequence memory game not implemented yet" });

                case GameTypes.CHIMP:
                    // Not Implemented
                    return StatusCode(501, new { Message = "Chimp test game not implemented yet" });

                default:
                    return BadRequest(new { Message = $"Unhandled game type: {gameType}" });
            }

            // Create the UserScore object and assign the current time to GameDate
            var userScore = new UserScore
            {
                Username = submission.Username,
                Score = score,
                GameType = gameType,
                GameDate = DateTime.UtcNow // Set the submission time here
            };

            await _userScoreService.SaveScoreAsync(userScore);

            return Ok(new { Message = "Score saved successfully", Score = score });
        }
    }
}
