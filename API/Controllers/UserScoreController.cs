using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserScoreController(LongNumberService service, UserScoreService userScoreService) : ControllerBase
    {
        private readonly LongNumberService _longNumberService = service;
        private readonly UserScoreService _userScoreService = userScoreService;

        [HttpGet("leaderboard/{gameType}")]
        public IActionResult GetLeaderboard(string gameType, int top = 10)
        {
            try
            {
                var leaderboard = _userScoreService.GetLeaderboard()
                                .Where(score => score.GameType == gameType)
                                .OrderByDescending(score => score.Score)
                                .Take(top)
                                .ToList();

                return Ok(leaderboard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching the leaderboard:" + ex);
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

            // enum usage
            switch (parsedGameType)
            {
                case GameTypes.LONG_NUMBER:
                    score = _longNumberService.CalculateScore(submission.Level);
                    break;

                case GameTypes.SEQUENCE:
                    // Not implemented yet
                    return StatusCode(501, new { Message = "Sequence memory game not implemented yet" });

                case GameTypes.CHIMP:
                    // Not implemented yet
                    return StatusCode(501, new { Message = "Chimp test game not implemented yet" });

                default:
                    return BadRequest(new { Message = $"Unhandled game type: {gameType}" });
            }


            var userScore = new UserScore
            {
                Username = submission.Username,
                Score = score,
                GameType = gameType
            };

            await _userScoreService.SaveScoreAsync(userScore);

            return Ok(new { Message = "Score saved successfully", Score = score });
        }

    }
}