using Microsoft.AspNetCore.Mvc;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Timers;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LongNumberController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LongNumberController(AppDbContext context)
        {
            _context = context;
        }

        // Endpoint to generate a sequence based on level
        [HttpGet("generate-sequence/{level}")]
        public IActionResult GenerateSequence(int level)
        {
            var random = new Random();
            int numberOfDigits = level; // Each level increases the number of digits by 1
            var sequence = Enumerable.Range(0, numberOfDigits).Select(_ => random.Next(0, 9)).ToArray();

            // Calculate time limit (start with 3 seconds, +1 second per level)
            int timeLimit = 3 + level - 1;

            return Ok(new { Sequence = sequence, TimeLimit = timeLimit });
        }

        // Endpoint to submit user score and check if they got the sequence right
        [HttpPost("submit-score")]
        public async Task<IActionResult> SubmitScore([FromBody] ScoreSubmission submission)
        {
            // Assume the correct sequence was generated on the client side or saved in session
            var correctSequence = submission.CorrectSequence; // Replace with actual storage/retrieval logic

            // Calculate score based on correct input
            int score = CalculateScore(submission.GuessedSequence, correctSequence, submission.Level);

            // Save the user score
            var userScore = new UserScore
            {
                Username = submission.Username,
                Score = score
            };

            _context.UserScores.Add(userScore);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Score saved successfully", Score = score });
        }

        private int CalculateScore(int[] guessed, int[] correct, int level)
        {
            // Each level passed gives the player 1 point
            bool isCorrect = guessed.SequenceEqual(correct);
            return isCorrect ? level : 0;
        }
    }

    public class ScoreSubmission
    {
        public string Username { get; set; }
        public int[] GuessedSequence { get; set; }
        public int[] CorrectSequence { get; set; }
        public int Level { get; set; }
    }
}
