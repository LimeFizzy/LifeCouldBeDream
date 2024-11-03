using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserScoreController(LongNumberService longNumberService, SequenceService sequenceService, UserScoreService userScoreService) : ControllerBase
    {
        private readonly LongNumberService _longNumberService = longNumberService;
        private readonly SequenceService _sequenceService = sequenceService;
        private readonly UserScoreService _userScoreService = userScoreService;

        [HttpGet("leaderboard/{gameType}")]
        public IActionResult GetLeaderboard(string gameType)
        {
            try
            {
                var leaderboard = _userScoreService.GetLeaderboard()
                                    .Where(score => score.GameType == gameType)
                                    .ToList();

                var sortedLeaderboard = leaderboard
                                        .OrderBy(us => us)      // Use of IComparable interface functional 
                                        .ToList();

                sortedLeaderboard.ForEach(us =>                 // Use of foreach
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
                    score = _sequenceService.CalculateScore(submission.Level);
                    break;

                case GameTypes.CHIMP:
                    // Not Implemented
                    return StatusCode(501, new { Message = "Chimp test game not implemented yet" });

                default:
                    return BadRequest(new { Message = $"Unhandled game type: {gameType}" });
            }

            var userScore = new UserScore
            {
                Username = submission.Username,
                Score = score,
                GameType = gameType,
                GameDate = DateTime.Now.ToString()
            };

            await _userScoreService.SaveScoreAsync(userScore);

            return Ok(new { Message = "Score saved successfully", Score = score });
        }
    }
}
