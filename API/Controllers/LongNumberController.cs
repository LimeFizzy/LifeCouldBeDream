using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Services;
using API.Extensions;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LongNumberController : ControllerBase
    {
        private readonly LongNumberService _service;

        public LongNumberController(LongNumberService service)
        {
            _service = service;
        }

        [HttpGet("generate-sequence/{level}")]
        public IActionResult GenerateSequence(int level)
        {
            var sequence = _service.GenerateSequence(level);
            while (!sequence.IsValidSequence(level))
            {
                sequence = _service.GenerateSequence(level);
            }
            int timeLimit = 3 + level - 1;

            return Ok(new { Sequence = sequence, TimeLimit = timeLimit });
        }


        [HttpPost("submit-score/{gameType}")]
        public async Task<IActionResult> SubmitScore([FromBody] ScoreSubmission submission, string gameType)
        {
            if (!submission.GuessedSequence.IsValidSequence(submission.Level))
            {
                return BadRequest(new { Message = "Invalid sequence." });
            }

            var correctSequence = submission.CorrectSequence;
            int score = _service.CalculateScore(submission.GuessedSequence, correctSequence, submission.Level);

            var userScore = new UserScore
            {
                Username = submission.Username,
                Score = score,
                GameType = gameType
            };

            await _service.SaveScoreAsync(userScore);

            return Ok(new { Message = "Score saved successfully", Score = score });
        }

    }
}
