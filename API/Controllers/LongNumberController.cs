using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Services;
using API.Extensions;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LongNumberController(LongNumberService service) : ControllerBase
    {
        private readonly LongNumberService _service = service;

        // Endpoint to generate a sequence based on level
        [HttpGet("generate-sequence/{level}")]
        public IActionResult GenerateSequence(int level)
        {
            var random = new Random();
            var sequence = random.GenerateRandomSequence(level);
            int timeLimit = 3 + level - 1;

            return Ok(new { Sequence = sequence, TimeLimit = timeLimit });
        }


        // Endpoint to submit user score and check if they got the sequence right
        [HttpPost("submit-score")]
        public async Task<IActionResult> SubmitScore([FromBody] ScoreSubmission submission)
        {
            if (!submission.GuessedSequence.IsValidSequence(submission.Level))
            {
                return BadRequest(new { Message = "Invalid sequence." });
            }

            var correctSequence = submission.CorrectSequence; // Replace with actual storage/retrieval logic
            int score = _service.CalculateScore(submission.GuessedSequence, correctSequence, submission.Level);

            // Save the user score
            var userScore = new UserScore
            {
                Username = submission.Username,
                Score = score
            };

            await _service.SaveScoreAsync(userScore);

            return Ok(new { Message = "Score saved successfully", Score = score });
        }

    }
}
