using Microsoft.AspNetCore.Mvc;
using API.Interfaces;
using API.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserScoreController : ControllerBase
    {
        private readonly ILongNumberService _longNumberService;
        private readonly ISequenceService _sequenceService;
        private readonly IUserScoreService _userScoreService;

        public UserScoreController(
            ILongNumberService longNumberService,
            ISequenceService sequenceService,
            IUserScoreService userScoreService)
        {
            _longNumberService = longNumberService ?? throw new ArgumentNullException(nameof(longNumberService));
            _sequenceService = sequenceService ?? throw new ArgumentNullException(nameof(sequenceService));
            _userScoreService = userScoreService ?? throw new ArgumentNullException(nameof(userScoreService));
        }

        [HttpGet("leaderboard/{gameType}")]
        public IActionResult GetLeaderboard(string gameType)
        {
            try
            {
                var leaderboard = _userScoreService.GetLeaderboard()
                                    .Where(score => score.GameType == gameType)
                                    .ToList();

                var sortedLeaderboard = leaderboard
                                        .OrderBy(us => us)
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
            catch (Exception)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching the leaderboard." });
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
            try
            {
                score = parsedGameType switch
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

                await _userScoreService.SaveScoreAsync(userScore);

                return Ok(new { Message = "Score saved successfully", Score = score });
            }
            catch (NotImplementedException ex)
            {
                return StatusCode(501, new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "An error occurred while submitting the score." });
            }
        }
    }
}
